using System.Text;
using System.Windows.Input;

namespace fKeys___Sender.HotkeyLogic
{

    public class Hotkey
    {
        public Key Key { get; }

        public ModifierKeys Modifiers { get; }

        public Hotkey(Key key, ModifierKeys modifiers)
        {
            Key = key;
            Modifiers = modifiers;
        }

        public int GetVirtualKey()
        {
            return KeyInterop.VirtualKeyFromKey(Key);
        }

        public uint GetVirtualModifiers()
        {
            uint MOD = 0x0000;
            if (Modifiers.HasFlag(ModifierKeys.Alt))
                MOD |= 0x0001;
            if (Modifiers.HasFlag(ModifierKeys.Control))
                MOD |= 0x0002;
            if (Modifiers.HasFlag(ModifierKeys.Shift))
                MOD |= 0x0004;
            if (Modifiers.HasFlag(ModifierKeys.Windows))
                MOD |= 0x0008;

            return MOD;
        }

        public override string ToString()
        {
            var str = new StringBuilder();

            if (Modifiers.HasFlag(ModifierKeys.Control))
                str.Append("Ctrl + ");
            if (Modifiers.HasFlag(ModifierKeys.Shift))
                str.Append("Shift + ");
            if (Modifiers.HasFlag(ModifierKeys.Alt))
                str.Append("Alt + ");
            if (Modifiers.HasFlag(ModifierKeys.Windows))
                str.Append("Win + ");

            str.Append(Key);

            return str.ToString();
        }
    }
}