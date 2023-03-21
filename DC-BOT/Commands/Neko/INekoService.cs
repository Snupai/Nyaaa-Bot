namespace DC_BOT.Commands.Neko
{
    internal interface INekoService
    {
        public string GetNeko(NekoKind kind);
    }

    internal enum NekoKind { 
        Neko,
        NekoBoy,
        NekoGif,
        NekoPara
    }
}
