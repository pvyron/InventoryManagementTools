using InMa.Shopping.Data.Repositories.Abstractions;
using Microsoft.AspNetCore.Components;

namespace InMa.Shopping.Components.Pages;

public partial class Home
{
    [Inject(Key = "Open")] public IListsRepository OpenListsRepository { get; set; } = null!;
}