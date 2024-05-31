using InMa.Shopping.Data.Repositories.Abstractions;
using InMa.Shopping.Data.Repositories.Models;
using InMa.Shopping.DomainExtensions;
using InMa.Shopping.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.FluentUI.AspNetCore.Components;

namespace InMa.Shopping.Components.FileSharing.Pages;

public partial class UploadFiles
{
    private readonly long _maxFileSizeInBytes = 5L * 1_024 * 1_024 * 1_024;
    
    [Inject] private IFilesRepository FilesRepository { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;
    
    private FluentInputFile? myFileByStream = default!;

    private SharedFileVm SharedFilesVm { get; set; } = new();
    private IBrowserFile[] _inputFiles = [];
    
    void OnFilesSelectedAsync(InputFileChangeEventArgs file)
    {
        try
        {
            _inputFiles = file.GetMultipleFiles().ToArray();

            SharedFilesVm.FileProperties =
                _inputFiles.Select((f, i) =>
                        new SharedFileInputProperties
                        {
                            Name = f.Name,
                            OriginalName = f.Name,
                            ContentType = f.ContentType,
                            FileSizeBytes = f.Size,
                            LastModified = f.LastModified
                        })
                    .ToArray();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
        }
    }

    async Task UploadAllButtonClicked(MouseEventArgs args)
    {
        try
        {
            var state = await AuthenticationStateProvider.GetAuthenticationStateAsync();

            var blobIds = await FilesRepository.UploadFiles(
                _inputFiles.Select(f => f.OpenReadStream(_maxFileSizeInBytes, CancellationToken.None)).ToArray(),
                new UploadFilesInfo
                {
                    UploaderEmail = state.User.Identity!.Name!,
                    CountryCode = SharedFilesVm.CountryCode,
                    Region = SharedFilesVm.Region,
                    City = SharedFilesVm.City,
                    DateCaptured = SharedFilesVm.DateCaptured.GetValueOrDefault(DateTime.UtcNow),
                    Tags = SharedFilesVm.Tags.Split(';').ToArray(),
                    SharedFileUsers = SharedFilesVm.ShareWith.Split(';'),
                    FilesProperties = SharedFilesVm.FileProperties.Select(fileProperties => fileProperties.GetFileUploadProperties()).ToArray()
                }, CancellationToken.None);

            SharedFilesVm.FileProperties = [];
            _inputFiles = [];
            
            var dialog = await DialogService.ShowSuccessAsync($"uploaded {blobIds.Count(id => !string.IsNullOrWhiteSpace(id))} files");
            var result = await dialog.Result;
            var cancelled = result.Cancelled;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
        }
    }
}