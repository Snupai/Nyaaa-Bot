namespace DC_BOT.Commands.Neko
{
    internal interface INekoService
    {
        public Task<string> GetNeko(NekoKind kind);
    }

    internal enum NekoKind { 
        Neko,
        NekoBoy,
        NekoGif,
        NekoPara
    }
}
