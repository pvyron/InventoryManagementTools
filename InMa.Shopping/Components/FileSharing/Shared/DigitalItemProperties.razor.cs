using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace InMa.Shopping.Components.FileSharing.Shared;

public partial class DigitalItemProperties
{
    [Parameter] public IBrowserFile DigitalItem { get; set; }
    [Parameter] public EventCallback<(IBrowserFile, int)> FilePropertiesChanged { get; set; }
    [Parameter] public EventCallback<(IBrowserFile, int)> UploadFileClicked { get; set; }
}