
namespace ZIP272X.Lora.Produced
{
    using System;

    public class BarCode
    {
        //Определение набора полос Code 128 по ID
        private static string Code_128_ID(int ID)
        {
            string S = "";
            switch (ID)
            {
                case 0: S = "212222"; break;
                case 1: S = "222122"; break;
                case 2: S = "222221"; break;
                case 3: S = "121223"; break;
                case 4: S = "121322"; break;
                case 5: S = "131222"; break;
                case 6: S = "122213"; break;
                case 7: S = "122312"; break;
                case 8: S = "132212"; break;
                case 9: S = "221213"; break;
                case 10: S = "221312"; break;
                case 11: S = "231212"; break;
                case 12: S = "112232"; break;
                case 13: S = "122132"; break;
                case 14: S = "122231"; break;
                case 15: S = "113222"; break;
                case 16: S = "123122"; break;
                case 17: S = "123221"; break;
                case 18: S = "223211"; break;
                case 19: S = "221132"; break;
                case 20: S = "221231"; break;
                case 21: S = "213212"; break;
                case 22: S = "223112"; break;
                case 23: S = "312131"; break;
                case 24: S = "311222"; break;
                case 25: S = "321122"; break;
                case 26: S = "321221"; break;
                case 27: S = "312212"; break;
                case 28: S = "322112"; break;
                case 29: S = "322211"; break;
                case 30: S = "212123"; break;
                case 31: S = "212321"; break;
                case 32: S = "232121"; break;
                case 33: S = "111323"; break;
                case 34: S = "131123"; break;
                case 35: S = "131321"; break;
                case 36: S = "112313"; break;
                case 37: S = "132113"; break;
                case 38: S = "132311"; break;
                case 39: S = "211313"; break;
                case 40: S = "231113"; break;
                case 41: S = "231311"; break;
                case 42: S = "112133"; break;
                case 43: S = "112331"; break;
                case 44: S = "132131"; break;
                case 45: S = "113123"; break;
                case 46: S = "113321"; break;
                case 47: S = "133121"; break;
                case 48: S = "313121"; break;
                case 49: S = "211331"; break;
                case 50: S = "231131"; break;
                case 51: S = "213113"; break;
                case 52: S = "213311"; break;
                case 53: S = "213131"; break;
                case 54: S = "311123"; break;
                case 55: S = "311321"; break;
                case 56: S = "331121"; break;
                case 57: S = "312113"; break;
                case 58: S = "312311"; break;
                case 59: S = "332111"; break;
                case 60: S = "314111"; break;
                case 61: S = "221411"; break;
                case 62: S = "431111"; break;
                case 63: S = "111224"; break;
                case 64: S = "111422"; break;
                case 65: S = "121124"; break;
                case 66: S = "121421"; break;
                case 67: S = "141122"; break;
                case 68: S = "141221"; break;
                case 69: S = "112214"; break;
                case 70: S = "112412"; break;
                case 71: S = "122114"; break;
                case 72: S = "122411"; break;
                case 73: S = "142112"; break;
                case 74: S = "142211"; break;
                case 75: S = "241211"; break;
                case 76: S = "221114"; break;
                case 77: S = "413111"; break;
                case 78: S = "241112"; break;
                case 79: S = "134111"; break;
                case 80: S = "111242"; break;
                case 81: S = "121142"; break;
                case 82: S = "121241"; break;
                case 83: S = "114212"; break;
                case 84: S = "124112"; break;
                case 85: S = "124211"; break;
                case 86: S = "411212"; break;
                case 87: S = "421112"; break;
                case 88: S = "421211"; break;
                case 89: S = "212141"; break;
                case 90: S = "214121"; break;
                case 91: S = "412121"; break;
                case 92: S = "111143"; break;
                case 93: S = "111341"; break;
                case 94: S = "131141"; break;
                case 95: S = "114113"; break;
                case 96: S = "114311"; break;
                case 97: S = "411113"; break;
                case 98: S = "411311"; break;
                case 99: S = "113141"; break;
                case 100: S = "114131"; break;
                case 101: S = "311141"; break;
                case 102: S = "411131"; break;
                case 103: S = "211412"; break;
                case 104: S = "211214"; break;
                case 105: S = "211232"; break;
                case 106: S = "2331112"; break;
            }
            return S;
        }

        //Определение набора полос Code 2 of 7 по символу
        private static string Codabar_Ch(String Ch)
        {
            String S = "";
            switch (Ch)
            {
                case "0": S = "11111331"; break;
                case "1": S = "11113311"; break;
                case "2": S = "11131131"; break;
                case "3": S = "33111111"; break;
                case "4": S = "11311311"; break;
                case "5": S = "31111311"; break;
                case "6": S = "13111131"; break;
                case "7": S = "13113111"; break;
                case "8": S = "13311111"; break;
                case "9": S = "31131111"; break;
                case "-": S = "11133111"; break;
                case "$": S = "11331111"; break;
                case ":": S = "31113131"; break;
                case "/": S = "31311131"; break;
                case ".": S = "31313111"; break;
                case "+": S = "11313131"; break;
                case "a": S = "11331111"; break;
                case "b": S = "13131131"; break;
                case "c": S = "11131331"; break;
                case "d": S = "11133311"; break;
                case "t": S = "11331311"; break;
                case "n": S = "13131131"; break;
                case "*": S = "11131331"; break;
                case "e": S = "11133311"; break;
            }
            return S;
        }

        //Определение набора полос Code 39 по символу
        private static string Code_39_Ch(String Ch)
        {
            String S = "";
            switch (Ch)
            {
                case "1": S = "3113111131"; break;
                case "2": S = "1133111131"; break;
                case "3": S = "3133111111"; break;
                case "4": S = "1113311131"; break;
                case "5": S = "3113311111"; break;
                case "6": S = "1133311111"; break;
                case "7": S = "1113113131"; break;
                case "8": S = "3113113111"; break;
                case "9": S = "1133113111"; break;
                case "0": S = "1113313111"; break;
                case "A": S = "3111131131"; break;
                case "B": S = "1131131131"; break;
                case "C": S = "3131131111"; break;
                case "D": S = "1111331131"; break;
                case "E": S = "3111331111"; break;
                case "F": S = "1131331111"; break;
                case "G": S = "1111133131"; break;
                case "H": S = "3111133111"; break;
                case "I": S = "1131133111"; break;
                case "J": S = "1111333111"; break;
                case "K": S = "3111111331"; break;
                case "L": S = "1131111331"; break;
                case "M": S = "3131111311"; break;
                case "N": S = "1111311331"; break;
                case "O": S = "3111311311"; break;
                case "P": S = "1131311311"; break;
                case "Q": S = "1111113331"; break;
                case "R": S = "3111113311"; break;
                case "S": S = "1131113311"; break;
                case "T": S = "1111313311"; break;
                case "U": S = "3311111131"; break;
                case "V": S = "1331111131"; break;
                case "W": S = "3331111111"; break;
                case "X": S = "1311311131"; break;
                case "Y": S = "3311311111"; break;
                case "Z": S = "1331311111"; break;
                case "-": S = "1311113131"; break;
                case ".": S = "3311113111"; break;
                case " ": S = "1331113111"; break;
                case "*": S = "1311313111"; break;
                case "$": S = "1313131111"; break;
                case "/": S = "1313111311"; break;
                case "+": S = "1311131311"; break;
                case "%": S = "1113131311"; break;
            }
            return S;
        }

        //Определение ширины полос Interleaved 2 of 5 для одного символа
        private static string Code_2of5_Ch(String Ch)
        {
            String S = "";
            switch (Ch)
            {
                case "0": S = "11331"; break;
                case "1": S = "31113"; break;
                case "2": S = "13113"; break;
                case "3": S = "33111"; break;
                case "4": S = "11313"; break;
                case "5": S = "31311"; break;
                case "6": S = "13311"; break;
                case "7": S = "11133"; break;
                case "8": S = "31131"; break;
                case "9": S = "13131"; break;
            }
            return S;
        }

        //Определение набора полос Interleaved 2 of 5 по двум символам
        private static string Interleaved_2of5_Pair(String Pair)
        {
            String S, S1, S2;

            S1 = Code_2of5_Ch(Pair.Substring(0, 1));
            S2 = Code_2of5_Ch(Pair.Substring(1, 1));
            S = "";
            for (int I = 0; I < S1.Length; I++)
                S = S + S1.Substring(I, 1) + S2.Substring(I, 1);
            return S;
        }

        //Штриховые символы шрифта iQs Code 128 по набору полос
        private static string Code_Char(String A)
        {
            String S;
            switch (A)
            {
                case "211412": S = "A"; break;
                case "211214": S = "B"; break;
                case "211232": S = "C"; break;
                case "2331112": S = "@"; break;
                default:
                    S = "";
                    for (int I = 0; I < A.Length / 2; I++)
                    {
                        switch (A.Substring(2 * I, 2))
                        {
                            case "11":
                                S = S + "0"; break;
                            case "21":
                                S = S + "1"; break;
                            case "31":
                                S = S + "2"; break;
                            case "41":
                                S = S + "3"; break;
                            case "12":
                                S = S + "4"; break;
                            case "22":
                                S = S + "5"; break;
                            case "32":
                                S = S + "6"; break;
                            case "42":
                                S = S + "7"; break;
                            case "13":
                                S = S + "8"; break;
                            case "23":
                                S = S + "9"; break;
                            case "33":
                                S = S + ":"; break;
                            case "43":
                                S = S + ";"; break;
                            case "14":
                                S = S + "<"; break;
                            case "24":
                                S = S + "="; break;
                            case "34":
                                S = S + ">"; break;
                            case "44":
                                S = S + "?"; break;
                        }
                    }
                    break;
            }
            return S;
        }

        //Строка штрих-кода в кодировке Code 128
        public static string Code_128(String A)
        {
            int[] BCode = new int[1024];
            int BInd, Ch, Ch2, I, LenA, CCode;
            string CurMode, S;
            //string[] BarArray;


            // Собираем строку кодов
            BInd = 0;
            CurMode = "";
            I = 0;
            LenA = A.Length;
            while (I < LenA)
            {
                // Текущий символ в строке
                Ch = Convert.ToInt32(A[I]);
                I++;
                // Разбираем только символы от 0 до 127
                if (Ch <= 127)
                {
                    // Следующий символ
                    if (I < LenA)
                        Ch2 = Convert.ToInt32(A[I]);
                    else
                        Ch2 = 0;
                    // Пара цифр - режим С
                    if ((48 <= Ch) && (Ch <= 57) && (48 <= Ch2) && (Ch2 <= 57))
                    {
                        I++;
                        if (BInd == 0)
                        {
                            //Начало с режима С
                            CurMode = "C";
                            BCode[BInd] = 105;
                            BInd = BInd + 1;
                        }
                        else
                        if (CurMode != "C")
                        {
                            // Переключиться на режим С
                            CurMode = "C";
                            BCode[BInd] = 99;
                            BInd = BInd + 1;
                        }
                        // Добавить символ режима С
                        string ttt = Convert.ToChar(Ch).ToString() + Convert.ToChar(Ch2);
                        BCode[BInd] = int.Parse(ttt);
                        BInd++;
                    }
                    else
                    {
                        if (BInd == 0)
                        {
                            if (Ch < 32)
                            {
                                //Начало с режима A
                                CurMode = "A";
                                BCode[BInd] = 103;
                                BInd++;
                            }
                            else
                            {
                                // Начало с режима B
                                CurMode = "B";
                                BCode[BInd] = 104;
                                BInd++;
                            }
                        }
                        // Переключение по надобности в режим A
                        if ((Ch < 32) && (CurMode != "A"))
                        {
                            CurMode = "A";
                            BCode[BInd] = 101;
                            BInd++;
                        }
                        // Переключение по надобности в режим B
                        else
                            if (((64 <= Ch) && (CurMode != "B")) || (CurMode == "C"))
                        {
                            CurMode = "B";
                            BCode[BInd] = 100;
                            BInd++;
                        }
                        // Служебные символы
                        if (Ch < 32)
                        {
                            BCode[BInd] = Ch + 64;
                            BInd++;
                        }
                        // Все другие символы
                        else
                        {
                            BCode[BInd] = Ch - 32;
                            BInd++;
                        }
                    }
                }
            }

            //Подсчитываем контрольную сумму
            CCode = BCode[0] % 103;
            for (int J = 1; J < BInd; J++)
                CCode = (CCode + BCode[J] * J) % 103;
            BCode[BInd] = CCode;
            BInd++;
            // Завершающий символ
            BCode[BInd] = 106;
            BInd++;
            //Собираем строку символов шрифта
            /*
                'BarArray = Array("155", "515", "551", "449", "485", "845", "458", "494", "854", _
                    "548", "584", "944", "056", "416", "452", "065", "425", "461", "560", "506", _
                    "542", "164", "524", "212", "245", "605", "641", "254", "614", "650", "119", _
                    "191", "911", "089", "809", "881", "098", "818", "890", "188", "908", "980", _
                    "01:", "092", "812", "029", "0:1", "821", "221", "182", "902", "128", "1:0", _
                    "122", "209", "281", ":01", "218", "290", ":10", "230", "5<0", ";00", "04=", _
                    "0<5", "40=", "4<1", "<05", "<41", "05<", "0=4", "41<", "4=0", "<14", "<50", _
                    "=40", "50<", "320", "=04", "830", "047", "407", "443", "074", "434", "470", _
                    "344", "704", "740", "113", "131", "311", "00;", "083", "803", "038", "0;0", _
                    "308", "380", "023", "032", "203", "302", "A", "B", "C", "@")
            */
            S = "";
            for (int J = 0; J < BInd; J++)
                S = S + Code_Char(Code_128_ID(BCode[J]));
            return S;
        }

        //Строка штрих-кода в кодировке Code 39
        public static string Code_39(String A)
        {
            string S;
            S = "";
            for (int I = 0; I < A.Length; I++)
                S = S + Code_Char(Code_39_Ch(A.Substring(I, 1)));
            //Старт/стоп - символ *
            return Code_Char(Code_39_Ch("*")) + S + Code_Char(Code_39_Ch("*"));
        }

        /*
        'Строка штрих-кода в кодировке Codabar
        Public Function Codabar(A As String)
            Dim I As Integer
            Dim S As String
            S = ""
            For I = 1 To Len(A)
                S = S & Code_Char(Codabar_Ch(Mid(A, I, 1)))
            Next I
            'Старт/стоп d/e. Возможные варианты a/t b/n c/* d/e
            Codabar = Code_Char(Codabar_Ch("d")) & S & Code_Char(Codabar_Ch("e"))
        End Function

        'Строка штрих-кода в кодировке Code 39
        Public Function Code_39(A As String)
            Dim I As Integer
            Dim S As String
            S = ""
            For I = 1 To Len(A)
                S = S & Code_Char(Code_39_Ch(Mid(A, I, 1)))
            Next I
            'Старт/стоп - символ *
            Code_39 = Code_Char(Code_39_Ch("*")) & S & Code_Char(Code_39_Ch("*"))
        End Function

        'Строка штрих-кода в кодировке Interleaved 2 of 5
        Public Function Interleaved_2of5(A As String, Optional Check As Boolean = False)
            Dim I As Integer
            Dim D As String
            Dim S As String
            Dim Ch As Integer
            Dim K As Integer
            'Преобразование к строке фифр
            D = ""
            For I = 1 To Len(A)
                Ch = Asc(Mid(A, I, 1))
                If(48 <= Ch) And(Ch <= 57) Then
                  D = D & Chr(Ch)
                End If
            Next I
            'Добавить лидирующий 0
            If((Len(D) Mod 2 > 0) And(Not Check)) Or _
              ((Len(D) Mod 2 = 0) And Check) Then
              D = "0" & D
            End If
            'Расчет и добавление контрольного разряда
            If Check Then
                K = 0
                For I = 1 To Len(D)
                    If I Mod 2 > 0 Then
                        K = K + CInt(Mid(D, I, 1)) * 3
                    Else
                        K = K + CInt(Mid(D, I, 1))
                    End If
                Next I
                K = 10 - (K Mod 10)
                D = D & CStr(K)
            End If
            'Составление строки кода по парам цифр
            S = ""
            For I = 0 To Len(D) / 2 - 1
                S = S & Code_Char(Interleaved_2of5_Pair(Mid(D, I* 2 + 1, 2)))
            Next I
            'Добавить старт/стоп символы
            Interleaved_2of5 = Code_Char("1111") & S & Code_Char("3111")
        End Function
        */
    }
}
