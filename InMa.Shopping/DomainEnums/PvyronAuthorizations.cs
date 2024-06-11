namespace InMa.Shopping.DomainEnums;

public enum PvyronAuthorizations : byte
{
    ShoppingLists = 1, //10000000
    ShoppingListsComplete = 2, //01000000
    FilesUpload = 4, //00100000
    PlaceHolder1 = 8, //00010000
    PlaceHolder2 = 16, //00001000
    PlaceHolder3 = 32, //00000100
    PlaceHolder4 = 64, //00000010
    PlaceHolder5 = 128, //00000001
}