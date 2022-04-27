using UnityEngine;
using System;


namespace TMPro
{
    [CreateAssetMenu]
    public class TMP_Validator : TMP_InputValidator
    {
        // Custom text input validation function
        public override char Validate(ref string text, ref int pos, char ch)
        {
            if (ch >= '0' && ch <= '9' || ch == '.')
            {
                text += ch;
                pos += 1;
                return ch;
            }
            return (char)0;
        }
    }
}