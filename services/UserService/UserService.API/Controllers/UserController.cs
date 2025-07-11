using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.DTOs;
using UserService.Application.Services;
using FluentValidation;
using ErrorOr;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace UserService.API.Controllers;

/// <summary>
/// Controlador para gerenciamento de usuários.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IValidationService _validationService;
    private readonly ILogger<UserController> _logger;
    private readonly IDistributedCache _cache;

    public UserController(IUserService userService, IValidationService validationService, ILogger<UserController> logger, IDistributedCache cache)
    {
        _userService = userService;
        _validationService = validationService;
        _logger = logger;
        _cache = cache;
    }

    /// <summary>
    /// Busca um usuário pelo ID.
    /// </summary>
    /// <param name="id">ID do usuário.</param>
    /// <returns>Retorna o usuário encontrado ou erro caso não exista.</returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDto>> GetById(Guid id)
    {
        var cacheKey = $"user:{id}";
        var cachedUser = await _cache.GetStringAsync(cacheKey);
        if (cachedUser != null)
        {
            var userDto = JsonSerializer.Deserialize<UserDto>(cachedUser);
            return Ok(userDto);
        }

        var result = await _userService.GetByIdAsync(id);
        return result.Match<ActionResult>(
            user => {
                // Salva no cache de forma síncrona (mas fora do Match)
                _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(user), new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                }).GetAwaiter().GetResult();
                return Ok(user);
            },
            erros => {
                var erro = erros.First();
                return erro.Type switch
                {
                    ErrorType.NotFound => NotFound(new { message = erro.Description }),
                    _ => StatusCode(500, new { message = erro.Description })
                };
            }
        );
    }

    /// <summary>
    /// Cria um novo usuário.
    /// </summary>
    /// <param name="createUserDto">Dados para criação do usuário.</param>
    /// <returns>Retorna o usuário criado ou erro de validação/conflito.</returns>
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserDto createUserDto)
    {
        // Validação customizada adicional
        var emailValidation = await _validationService.ValidateUserEmailAsync(createUserDto.Email);
        if (!emailValidation.IsValid)
        {
            return BadRequest(new
            {
                Success = false,
                Message = "Erro de validação",
                Errors = emailValidation.Errors.Select(e => e.ErrorMessage)
            });
        }

        var result = await _userService.CreateAsync(createUserDto);
        return result.Match<ActionResult>(
            user => CreatedAtAction(nameof(GetById), new { id = user.Id }, user),
            erros => {
                var erro = erros.First();
                return erro.Type switch
                {
                    ErrorType.Conflict => Conflict(new { message = erro.Description }),
                    ErrorType.Validation => BadRequest(new { message = erro.Description }),
                    _ => StatusCode(500, new { message = erro.Description })
                };
            }
        );
    }

    /// <summary>
    /// Lista todos os usuários.
    /// </summary>
    /// <returns>Retorna a lista de usuários.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
    {
        var result = await _userService.GetAllAsync();
        return result.Match<ActionResult>(
            users => Ok(users),
            erros => {
                var erro = erros.First();
                return StatusCode(500, new { message = erro.Description });
            }
        );
    }

    /// <summary>
    /// Lista as contas sociais conectadas do usuário.
    /// </summary>
    [HttpGet("{userId:guid}/social-accounts")]
    public async Task<ActionResult<List<SocialAccountDto>>> GetSocialAccounts(Guid userId)
    {
        var result = await _userService.GetSocialAccountsAsync(userId);
        return Ok(result);
    }

    /// <summary>
    /// Conecta uma conta social ao usuário.
    /// </summary>
    [HttpPost("{userId:guid}/social-accounts")]
    public async Task<ActionResult<SocialAccountDto>> ConnectSocialAccount(Guid userId, [FromBody] ConnectSocialAccountDto dto)
    {
        var result = await _userService.ConnectSocialAccountAsync(userId, dto);
        return result.Match<ActionResult>(
            ok => Ok(ok),
            erros => {
                var erro = erros.First();
                return erro.Type switch
                {
                    ErrorType.Conflict => Conflict(new { message = erro.Description }),
                    _ => StatusCode(500, new { message = erro.Description })
                };
            }
        );
    }

    /// <summary>
    /// Desconecta uma conta social do usuário.
    /// </summary>
    [HttpDelete("{userId:guid}/social-accounts/{networkType}")]
    public async Task<ActionResult> DisconnectSocialAccount(Guid userId, SocialNetworkTypeDto networkType)
    {
        var result = await _userService.DisconnectSocialAccountAsync(userId, networkType);
        return result.Match<ActionResult>(
            ok => NoContent(),
            erros => {
                var erro = erros.First();
                return erro.Type switch
                {
                    ErrorType.NotFound => NotFound(new { message = erro.Description }),
                    _ => StatusCode(500, new { message = erro.Description })
                };
            }
        );
    }
} 