using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.DTOs;
using UserService.Application.Services;

namespace UserService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDto>> GetById(Guid id)
    {
        try
        {
            var user = await _userService.GetByIdAsync(id);
            
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", id);
                return NotFound($"User with ID {id} not found");
            }

            _logger.LogInformation("User with ID {UserId} retrieved successfully", id);
            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with ID {UserId}", id);
            return StatusCode(500, "An error occurred while retrieving the user");
        }
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserDto createUserDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userService.CreateAsync(createUserDto);
            
            _logger.LogInformation("User created successfully with ID {UserId}", user.Id);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to create user with email {Email}", createUserDto.Email);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user with email {Email}", createUserDto.Email);
            return StatusCode(500, "An error occurred while creating the user");
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
    {
        try
        {
            var users = await _userService.GetAllAsync();
            _logger.LogInformation("Retrieved {Count} users", users.Count());
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all users");
            return StatusCode(500, "An error occurred while retrieving users");
        }
    }
} 