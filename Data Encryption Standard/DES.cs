using System;
using System.Text;

namespace Data_Encryption_Standard
{
    class DES
    {
        private static char[] initDataBytesArray, key64;
        private static char[,] arrayBlocksData;

        #region -----Таблицы DES-----
        /// <summary>
        /// Массив начальной перестановки
        /// </summary>
        private static int[] initPermutation = {
            58, 50, 42, 34, 26, 18, 10, 2,
            60, 52, 44, 36, 28, 20, 12, 4,
            62, 54, 46, 38, 30, 22, 14, 6,
            64, 56, 48, 40, 32, 24, 16, 8,
            57, 49, 41, 33, 25, 17, 9, 1,
            59, 51, 43, 35, 27, 19, 11, 3,
            61, 53, 45, 37, 29, 21, 13, 5,
            63, 55, 47, 39, 31, 23, 15, 7
        };

        /// <summary>
        /// Массив конечной перестановки
        /// </summary>
        private static int[] finalPermutation = {
            40, 8, 48, 16, 56, 24, 64, 32,
            39, 7, 47, 15, 55, 23, 63, 31,
            38, 6, 46, 14, 54, 22, 62, 30,
            37, 5, 45, 13, 53, 21, 61, 29,
            36, 4, 44, 12, 52, 20, 60, 28,
            35, 3, 43, 11, 51, 19, 59, 27,
            34, 2, 42, 10, 50, 18, 58, 26,
            33, 1, 41, 9, 49, 17, 57, 25
        };

        /// <summary>
        /// Массив начальной подготовки ключа
        /// </summary>
        private static int[] initKeyPreparation = {
            57, 49, 41, 33, 25, 17, 9,
            1, 58, 50, 42, 34, 26, 18,
            10, 2, 59, 51, 43, 35, 27,
            19, 11, 3, 60, 52, 44, 36,
            63, 55, 47, 39, 31, 23, 15,
            7, 62, 54, 46, 38, 30, 22,
            14, 6, 61, 53, 45, 37, 29,
            21, 13, 5, 28, 20, 12, 4
        };

        /// <summary>
        /// Массив конечной подготовки ключа
        /// </summary>
        private static int[] finalKeyPrepation = {
            14, 17, 11, 24, 1, 5,
            3, 28, 15, 6, 21, 10,
            23, 19, 12, 4, 26, 8,
            16, 7, 27, 20, 13, 2,
            41, 52, 31, 37, 47, 55,
            30, 40, 51, 45, 33, 48,
            44, 49, 39, 56, 34, 53,
            46, 42, 50, 36, 29, 32
        };

        /// <summary>
        /// Массив сдвигов для вычисления ключа
        /// </summary>
        private static int[] shifts = { 1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1 };

        /// <summary>
        /// Массив для функции расширения
        /// </summary>
        private static int[] extensionMass = {
            32, 1, 2, 3, 4, 5,
            4, 5, 6, 7, 8, 9,
            8,  9, 10, 11, 12, 13,
            12, 13, 14, 15, 16, 17,
            16, 17, 18, 19, 20, 21,
            20, 21, 22, 23, 24, 25,
            24, 25, 26, 27, 28, 29,
            28, 29, 30, 31, 32, 1
        };

        private static int[,,] permutationS = {
            {
                {14, 4, 13, 1, 2, 15, 11, 8, 3, 10, 6, 12, 5, 9, 0, 7},
                {0, 15, 7, 4, 14, 2, 13, 1, 10, 6, 12, 11, 9, 5, 3, 8},
                {4, 1, 14, 8, 13, 6, 2, 11, 15, 12, 9, 7, 3, 10, 5, 0},
                {15, 12, 8, 2, 4, 9, 1, 7, 5, 11, 3, 14, 10, 0, 6, 13}
            },
            {
                {15, 1, 8, 14, 6, 11, 3, 4, 9, 7, 2, 13, 12, 0, 5, 10},
                {3, 13, 4, 7, 15, 2, 8, 14, 12, 0, 1, 10, 6, 9, 11, 5},
                {0, 14, 7, 11, 10, 4, 13, 1, 5, 8, 12, 6, 9, 3, 2, 15},
                {13, 8, 10, 1, 3, 15, 4, 2, 11, 6, 7, 12, 0, 5, 14, 9}
            },
            {
                {10, 0, 9, 14, 6, 3, 15, 5, 1, 13, 12, 7, 11, 4, 2, 8},
                {13, 7, 0, 9, 3, 4, 6, 10, 2, 8, 5, 14, 12, 11, 15, 1},
                {13, 6, 4, 9, 8, 15, 3, 0, 11, 1, 2, 12, 5, 10, 14, 7},
                {1, 10, 13, 0, 6, 9, 8, 7, 4, 15, 14, 3, 11, 5, 2, 12}
            },
            {
                {7, 13, 14, 3, 0, 6, 9, 10, 1, 2, 8, 5, 11, 12, 4, 15 },
                {13, 8, 11, 5, 6, 15, 0, 3, 4, 7, 2, 12, 1, 10, 14, 9 },
                {10, 6, 9, 0, 12, 11, 7, 13, 15, 1, 3, 14, 5, 2, 8, 4 },
                {3, 15, 0, 6, 10, 1, 13, 8, 9, 4, 5, 11, 12, 7, 2, 14 }
            },
            {
                {2, 12, 4, 1, 7, 10, 11, 6, 8, 5, 3, 15, 13, 0, 14, 9 },
                {14, 11, 2, 12, 4, 7, 13, 1, 5, 0, 15, 10, 3, 9, 8, 6 },
                {4, 2, 1, 11, 10, 13, 7, 8, 15, 9, 12, 5, 6, 3, 0, 14 },
                {11, 8, 12, 7, 1, 14, 2, 13, 6, 15, 0, 9, 10, 4, 5, 3 }
            },
            {
                {12, 1, 10, 15, 9, 2, 6, 8, 0, 13, 3, 4, 14, 7, 5, 11 },
                {10, 15, 4, 2, 7, 12, 9, 5, 6, 1, 13, 14, 0, 11, 3, 8 },
                {9, 14, 15, 5, 2, 8, 12, 3, 7, 0, 4, 10, 1, 13, 11, 6 },
                {4, 3, 2, 12, 9, 5, 15, 10, 11, 14, 1, 7, 6, 0, 8, 13 }
            },
            {
                {4, 11, 2, 14, 15, 0, 8, 13, 3, 12, 9, 7, 5, 10, 6, 1 },
                {13, 0, 11, 7, 4, 9, 1, 10, 14, 3, 5, 12, 2, 15, 8, 6 },
                {1, 4, 11, 13, 12, 3, 7, 14, 10, 15, 6, 8, 0, 5, 9, 2 },
                {6, 11, 13, 8, 1, 4, 10, 7, 9, 5, 0, 15, 14, 2, 3, 12 }
            },
            {
                {13, 2, 8, 4, 6, 15, 11, 1, 10, 9, 3, 14, 5, 0, 12, 7 },
                {1, 15, 13, 8, 10, 3, 7, 4, 12, 5, 6, 11, 0, 14, 9, 2 },
                {7, 11, 4, 1, 9, 12, 14, 2, 0, 6, 10, 13, 15, 3, 5, 8 },
                {2, 1, 14, 7, 4, 10, 8, 13, 15, 12, 9, 0, 3, 5, 6, 11 }
            }
        };

        private static int[] permutationP = {
            16, 7, 20, 21,
            29, 12, 28, 17,
            1, 15, 23, 26,
            5, 18, 31, 10,
            2, 8, 24, 14,
            32, 27, 3, 9,
            19, 13, 30, 6,
            22, 11, 4, 25
        };
        #endregion

        /// <summary>
        /// Шифровка текста.
        /// </summary>
        /// <param name="openData">Открытый текст</param>
        /// <param name="key">Ключ</param>
        public static string EncryptDES(string openData, string key, out byte[] closeByteText)
        {


            initDataBytesArray = ConvertToBinaryString(openData).ToCharArray();           // Данные в битовом представлении
            key64 = AddTestBits(ConvertToBinaryString(key, "ASСII").ToCharArray());       // Побитовый ключ с проверочными на нечётность битами
            arrayBlocksData = DataToBlocksArray(initDataBytesArray);                      // Разбивка данных на массив блоков
            arrayBlocksData = InitialPermutation(arrayBlocksData);                        // Начальная перестановка в блоках
            arrayBlocksData = MainPermutation(arrayBlocksData);                           // Прямое преобразование Фейстеля
            arrayBlocksData = FinalPermutation(arrayBlocksData);                          // Финальная перестановка

            closeByteText = ConvertToArrayBytes(arrayBlocksData);
            return Encoding.Default.GetString(closeByteText);
        }
        /// <summary>
        /// Расшифровка закодированного методом DES текста
        /// </summary>
        /// <param name="closeData">Закрытый текст</param>
        /// <param name="key">Ключ</param>
        /// <returns></returns>
        public static string DecryptDES(byte[] closeByteData, string key)
        {
            initDataBytesArray = ConvertToBinaryString(closeByteData).ToCharArray();      // Данные в битовом представлении
            key64 = AddTestBits(ConvertToBinaryString(key, "ASСII").ToCharArray());       // Побитовый ключ с проверочными на нечётность битами
            arrayBlocksData = DataToBlocksArray(initDataBytesArray);                // Разбивка данных на массив блоков
            arrayBlocksData = InitialPermutation(arrayBlocksData);                  // Начальная перестановка в блоках
            arrayBlocksData = ReversePermutation(arrayBlocksData);                  // Обратное преобразование Фейстеля
            arrayBlocksData = FinalPermutation(arrayBlocksData);                    // Финальная перестановка

            byte[] openByteData = ConvertToArrayBytes(arrayBlocksData);
            return Encoding.Default.GetString(openByteData);
        }

        /// <summary>
        /// Основные перестановки
        /// </summary>
        /// <param name="blocksData"></param>
        private static char[,] MainPermutation(char[,] blocksData)
        {
            char[,] blocksDataModified = new char[blocksData.GetLength(0), blocksData.GetLength(1)];
            char[] L32 = new char[32];
            char[] R32 = new char[32];
            char[] f32, bufR32;
            char[][] arrayKey48 = PreparingKey(key64);

            // Разбиение блока на две части по 32 битов
            for (var k = 0; k < blocksData.GetLength(0); k++)
            {
                for (var i = 0; i < L32.Length; i++)
                {
                    L32[i] = blocksData[k, i];
                    R32[i] = blocksData[k, i + L32.Length];
                }
                for (var i = 0; i < 16; i++)
                {
                    f32 = PermutationSandP(XoR(ExtensionFunctionE(R32), arrayKey48[i]));        // Перестановка S и P блока В = R48^Key48
                    // Перестановка R32 c L32
                    bufR32 = R32;
                    R32 = XoR(L32, f32);
                    L32 = bufR32;
                }

                for (var i = 0; i < L32.Length; i++)
                {
                    blocksDataModified[k, i] = L32[i];
                    blocksDataModified[k, i + 32] = R32[i];
                }
            }

            return blocksDataModified;
        }

        private static char[,] ReversePermutation(char[,] blocksData)
        {
            char[,] blocksDataModified = new char[blocksData.GetLength(0), blocksData.GetLength(1)];
            char[] L32 = new char[32];
            char[] R32 = new char[32];
            char[] f32, bufL32;
            char[][] arrayKey48 = PreparingKey(key64);

            // Разбиение блока на две части по 32 битов
            for (var k = 0; k < blocksData.GetLength(0); k++)
            {
                for (var i = 0; i < L32.Length; i++)
                {
                    L32[i] = blocksData[k, i];
                    R32[i] = blocksData[k, i + L32.Length];
                }

                for (var i = 15; i >= 0; i--)
                {
                    f32 = PermutationSandP(XoR(ExtensionFunctionE(L32), arrayKey48[i]));        // Перестановка S и P
                    // Перестановка R32 c L32
                    bufL32 = L32;
                    L32 = XoR(R32, f32);
                    R32 = bufL32;
                }

                for (var i = 0; i < L32.Length; i++)
                {
                    blocksDataModified[k, i] = L32[i];
                    blocksDataModified[k, i + 32] = R32[i];
                }
            }

            return blocksDataModified;
        }

        /// <summary>
        /// Начальная перестановка.
        /// </summary>
        /// <param name="arrayBlocks"></param>
        private static char[,] InitialPermutation(char[,] arrayBlocks)
        {
            char[,] tempArrayBlocks = new char[arrayBlocks.GetLength(0), arrayBlocks.GetLength(1)];
            for (var i = 0; i < arrayBlocks.GetLength(0); i++)
                for (var j = 0; j < arrayBlocks.GetLength(1); j++)
                    tempArrayBlocks[i, j] = arrayBlocks[i, initPermutation[j] - 1];

            return tempArrayBlocks;
        }

        /// <summary>
        /// Начальная перестановка.
        /// </summary>
        /// <param name="arrayBlocks"></param>
        private static char[,] FinalPermutation(char[,] arrayBlocks)
        {
            char[,] tempArrayBlocks = new char[arrayBlocks.GetLength(0), arrayBlocks.GetLength(1)];
            for (var i = 0; i < arrayBlocks.GetLength(0); i++)
                for (var j = 0; j < arrayBlocks.GetLength(1); j++)
                    tempArrayBlocks[i, j] = arrayBlocks[i, finalPermutation[j] - 1];

            return tempArrayBlocks;
        }

        /// <summary>
        /// Функция расширения Е. ( R32 -> R48)
        /// </summary>
        /// <param name="R32">32-битовая последовательность</param>
        /// <returns></returns>
        private static char[] ExtensionFunctionE(char[] R32)
        {
            char[] R48 = new char[48];
            for (var i = 0; i < 48; i++)
                R48[i] = R32[extensionMass[i] - 1];
            return R48;
        }

        /// <summary>
        /// Подготовка ключа (64 бит -> 48 бит).
        /// </summary>
        /// <param name="key64">64-битный ключ</param>
        /// <returns></returns>
        private static char[][] PreparingKey(char[] key64)
        {
            char[][] arrayKey48 = new char[16][];
            for (var i = 0; i < arrayKey48.Length; i++)
                arrayKey48[i] = new char[48];

            int[] massKeyPreparation = new int[initKeyPreparation.Length];
            for (var i = 0; i < initKeyPreparation.Length; i++)
                massKeyPreparation[i] = initKeyPreparation[i];
            for (var iter = 0; iter < 16; iter++)
            {
                if (iter != 0)
                    for (var i = 0; i < 28; i++)    // Циклический сдвиг таблицы
                    {
                        massKeyPreparation[i] = initKeyPreparation[(i - shifts[iter] < 0) ? i - shifts[iter] + 28 : i - shifts[iter]];
                        massKeyPreparation[i + 28] = initKeyPreparation[(i - shifts[iter] < 0) ? i - shifts[iter] + 56 : i - shifts[iter] + 28];
                    }
                char[] key56 = new char[56];
                for (var i = 0; i < 56; i++)
                    key56[i] = key64[massKeyPreparation[i]];

                char[] key48 = new char[48];
                for (var i = 0; i < 48; i++)
                    key48[i] = key56[finalKeyPrepation[i] - 1];
            }


            return arrayKey48;
        }

        /// <summary>
        /// Побитовое сложение битовой последовательности (A|B = C).
        /// </summary>
        /// <param name="A">1-ая битовая последовательность</param>
        /// <param name="B">2-ая битовая последовательность</param>
        /// <returns></returns>
        private static char[] XoR(char[] A, char[] B)
        {
            char[] C = new char[A.Length];
            for (var i = 0; i < A.Length; i++)
                C[i] = (char)((A[i] + B[i]) % 2 + 48);
            return C;
        }

        /// <summary>
        /// Добавление в ключ битов с проверкой на нечётность. 
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <returns></returns>
        private static char[] AddTestBits(char[] key)
        {
            var sum = 0;
            for (var i = key.Length - 1; i >= 0; i--)
            {
                sum += Convert.ToInt32(key[i]);
                if (i % 8 == 0 || i == 0)
                {
                    key[i] = (sum % 2 == 0) ? '1' : '0';
                    sum = 0;
                }
            }
            return key;
        }

        /// <summary>
        /// Функция шифрования f(R48,Key48).
        /// </summary>
        /// <param name="B">Результат побитого сложения по модулю 2 R48 и Key48</param>
        /// <returns></returns>
        private static char[] PermutationSandP(char[] B)
        {
            char[] S = new char[32];
            for (var i = 0; i < 8; i++)
            {
                var S2 = B[i * 6 + 5] - 48 + (B[i * 6] - 48) * 2;                                                            // Перевод из двоичной в десятичную
                var S3 = B[i * 6 + 4] - 48 + (B[i * 6 + 3] - 48) * 2 + (B[i * 6 + 2] - 48) * 4 + (B[i * 6 + 1] - 48) * 8;    // Перевод из двоичной в десятичную
                for (var j = 0; j < 4; j++)
                    S[i * 4 + j] = Convert.ToString(permutationS[i, S2, S3], 2).PadLeft(4, '0').ToCharArray()[j];
            }
            char[] P = new char[32];
            for (var i = 0; i < P.Length; i++)
                P[i] = S[permutationP[i] - 1];

            return P;
        }

        /// <summary>
        /// Разбиение данных на массив блоков по 64 бит.
        /// </summary>
        /// <param name="initData">Входные данные</param>
        /// <returns></returns>
        private static char[,] DataToBlocksArray(char[] initData)
        {
            if (initData.Length % 64 != 0)
                initData = AddNullSymbols(initData);                    // Дополнение последовательности нулями для кратности длины 64-ём
            char[,] blocksData = new char[initData.Length / 64, 64];

            for (var i = 0; i < blocksData.GetLength(0); i++)
                for (var j = 0; j < 64; j++)
                    blocksData[i, j] = initData[i * 64 + j];

            return blocksData;
        }

        /// <summary>
        /// Добавление битов для кратности длины текста к 64.
        /// </summary>
        /// <param name="data">Исходный текст</param>
        /// <returns></returns>
        private static char[] AddNullSymbols(char[] data)
        {
            var lenght = data.Length;
            int reslen = lenght + (64 - lenght % 64);
            char[] dataRes = new char[reslen];
            for (var i = 0; i < reslen; i++)
                dataRes[i] = (i < lenght) ? data[i] : '0';

            return dataRes;
        }

        /// <summary>
        /// Конвертирует строку в битовую последовательность.
        /// </summary>
        /// <param name="data">Входные данные</param>
        /// <returns></returns>
        public static string ConvertToBinaryString(string data, string encod = "default")
        {
            byte[] arrayBytes = (encod == "default") ? Encoding.Default.GetBytes(data) : Encoding.ASCII.GetBytes(data);
            var str = new StringBuilder("");

            for (var i = 0; i < data.Length; i++)
                str.Append(Convert.ToString(arrayBytes[i], 2).PadLeft(8, '0'));

            return str.ToString();
        }

        /// <summary>
        /// Конвертирует строку в битовую последовательность.
        /// </summary>
        /// <param name="data">Входные данные</param>
        /// <returns></returns>
        public static string ConvertToBinaryString(byte[] data)
        {
            var str = new StringBuilder("");

            for (var i = 0; i < data.Length; i++)
                str.Append(Convert.ToString(data[i], 2).PadLeft(8, '0'));

            return str.ToString();
        }

        /// <summary>
        /// Конвертация массива byte в string
        /// </summary>
        /// <param name="blocksData"></param>
        /// <returns></returns>
        private static byte[] ConvertToArrayBytes(char[,] blocksData)
        {
            char[] closeBinaryText = new char[blocksData.Length];
            for (var i = 0; i < blocksData.GetLength(0); i++)
                for (var j = 0; j < blocksData.GetLength(1); j++)
                    closeBinaryText[i * 64 + j] = blocksData[i, j];

            byte[] byteText = new byte[closeBinaryText.Length / 8];
            for (var i = 0; i < byteText.Length; i++)
            {
                string closeText = "";
                for (var j = 0; j < 8; j++)
                    closeText += closeBinaryText[i * 8 + j];

                byteText[i] = (byte)Convert.ToInt32(closeText, 2);
            }

            return byteText;
        }
    }
}