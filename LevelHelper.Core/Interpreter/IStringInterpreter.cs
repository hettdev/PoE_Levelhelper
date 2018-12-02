namespace LevelHelper.Core.Interpreter
{
    interface IStringInterpreter
    {
        InterpretEventArgs InterpretLine(string line);
    }
}