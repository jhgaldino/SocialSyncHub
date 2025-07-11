using System.Net.Http.Json;
using UserService.Application.DTOs;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;

namespace UserService.Application.Services;

public class InstagramSyncService : IInstagramSyncService
{
    private readonly ISocialAccountRepository _socialAccountRepository;
    private readonly IInstagramMediaRepository _mediaRepository;
    private readonly HttpClient _httpClient;

    public InstagramSyncService(ISocialAccountRepository socialAccountRepository, IInstagramMediaRepository mediaRepository, HttpClient httpClient)
    {
        _socialAccountRepository = socialAccountRepository;
        _mediaRepository = mediaRepository;
        _httpClient = httpClient;
    }

    public async Task<List<InstagramMediaDto>> SyncUserMediaAsync(Guid userId)
    {
        var account = await _socialAccountRepository.GetByUserAndNetworkAsync(userId, SocialNetworkType.Instagram);
        if (account == null)
            throw new InvalidOperationException("Instagram account not connected.");

        var url = $"https://graph.instagram.com/me/media?fields=id,media_type,media_url,caption,timestamp&access_token={account.AccessToken}";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadFromJsonAsync<InstagramMediaApiResponse>();
        var result = new List<InstagramMediaDto>();
        if (json?.Data != null)
        {
            foreach (var media in json.Data)
            {
                var entity = new InstagramMedia
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    MediaId = media.Id,
                    MediaType = media.MediaType,
                    MediaUrl = media.MediaUrl,
                    Caption = media.Caption,
                    Timestamp = media.Timestamp
                };
                await _mediaRepository.AddOrUpdateAsync(entity);
                result.Add(new InstagramMediaDto
                {
                    MediaId = media.Id,
                    MediaType = media.MediaType,
                    MediaUrl = media.MediaUrl,
                    Caption = media.Caption,
                    Timestamp = media.Timestamp
                });
            }
        }
        return result;
    }

    private class InstagramMediaApiResponse
    {
        public List<InstagramMediaItem> Data { get; set; } = new();
    }
    private class InstagramMediaItem
    {
        public string Id { get; set; } = string.Empty;
        public string MediaType { get; set; } = string.Empty;
        public string MediaUrl { get; set; } = string.Empty;
        public string? Caption { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
