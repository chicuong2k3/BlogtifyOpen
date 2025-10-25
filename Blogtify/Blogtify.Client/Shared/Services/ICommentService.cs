using Blogtify.Client.Shared.Comments;
using System.Net.Http.Json;

namespace Blogtify.Client.Shared.Services;

public interface ICommentService
{
    Task<CommentDto?> GetByIdAsync(Guid id);
    Task<CommentDto> Create(CommentDto comment);
}

internal class CommentService : ICommentService
{
    private readonly HttpClient _httpClient;

    public CommentService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<CommentDto> Create(CommentDto comment)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/comments", comment);
        response.EnsureSuccessStatusCode();

        var created = await response.Content.ReadFromJsonAsync<CommentDto>();
        return created!;
    }

    public async Task<CommentDto?> GetByIdAsync(Guid id)
    {
        return await _httpClient.GetFromJsonAsync<CommentDto>($"/api/comments/{id}");
    }
}