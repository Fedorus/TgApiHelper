using System;
using System.Collections.Generic;

namespace TelegramFaqBotHost.TelegramFaq
{
    public static class StringExtensions
    {
        #region Работа со строками

        /// <summary>
        /// Извлекает подстроку из строки. Подстрока начинается с конца позиции подстроки <paramref name="left"/> и до конца строки. Поиск начинается с заданной позиции.
        /// </summary>
        /// <param name="str">Строка, в которой будет поиск подстроки.</param>
        /// <param name="left">Строка, которая находится слева от искомой подстроки.</param>
        /// <param name="startIndex">Позиция, с которой начинается поиск подстроки. Отсчёт от 0.</param>
        /// <param name="comparsion">Одно из значений перечисления, определяющее правила поиска.</param>
        /// <returns>Найденая подстрока, иначе пустая строка.</returns>
        /// <exception cref="System.ArgumentNullException">Значение параметра <paramref name="left"/> равно <see langword="null"/>.</exception>
        /// <exception cref="System.ArgumentException">Значение параметра <paramref name="left"/> является пустой строкой.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Значение параметра <paramref name="startIndex"/> меньше 0.
        /// -или-
        /// Значение параметра <paramref name="startIndex"/> равно или больше длины строки <paramref name="str"/>.
        /// </exception>
        public static string Substring(this string str, string left,
            int startIndex, StringComparison comparsion = StringComparison.Ordinal)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            #region Проверка параметров

            if (left == null)
            {
                throw new ArgumentNullException("left");
            }
            

            #endregion

            // Ищем начало позиции левой подстроки.
            int leftPosBegin = str.IndexOf(left, startIndex, comparsion);

            if (leftPosBegin == -1)
            {
                return string.Empty;
            }

            // Вычисляем конец позиции левой подстроки.
            int leftPosEnd = leftPosBegin + left.Length;

            // Вычисляем длину найденной подстроки.
            int length = str.Length - leftPosEnd;

            return str.Substring(leftPosEnd, length);
        }

        /// <summary>
        /// Извлекает подстроку из строки. Подстрока начинается с конца позиции подстроки <paramref name="left"/> и до конца строки.
        /// </summary>
        /// <param name="str">Строка, в которой будет поиск подстроки.</param>
        /// <param name="left">Строка, которая находится слева от искомой подстроки.</param>
        /// <param name="comparsion">Одно из значений перечисления, определяющее правила поиска.</param>
        /// <returns>Найденая подстрока, иначе пустая строка.</returns>
        /// <exception cref="System.ArgumentNullException">Значение параметра <paramref name="left"/> равно <see langword="null"/>.</exception>
        /// <exception cref="System.ArgumentException">Значение параметра <paramref name="left"/> является пустой строкой.</exception>
        public static string Substring(this string str,
            string left, StringComparison comparsion = StringComparison.Ordinal)
        {
            return Substring(str, left, 0, comparsion);
        }

        /// <summary>
        /// Извлекает подстроку из строки. Подстрока ищется между двумя заданными строками, начиная с заданной позиции.
        /// </summary>
        /// <param name="str">Строка, в которой будет поиск подстроки.</param>
        /// <param name="left">Строка, которая находится слева от искомой подстроки.</param>
        /// <param name="right">Строка, которая находится справа от искомой подстроки.</param>
        /// <param name="startIndex">Позиция, с которой начинается поиск подстроки. Отсчёт от 0.</param>
        /// <param name="comparsion">Одно из значений перечисления, определяющее правила поиска.</param>
        /// <returns>Найденая подстрока, иначе пустая строка.</returns>
        /// <exception cref="System.ArgumentNullException">Значение параметра <paramref name="left"/> или <paramref name="right"/> равно <see langword="null"/>.</exception>
        /// <exception cref="System.ArgumentException">Значение параметра <paramref name="left"/> или <paramref name="right"/> является пустой строкой.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Значение параметра <paramref name="startIndex"/> меньше 0.
        /// -или-
        /// Значение параметра <paramref name="startIndex"/> равно или больше длины строки <paramref name="str"/>.
        /// </exception>
        public static string Substring(this string str, string left, string right,
            int startIndex, StringComparison comparsion = StringComparison.Ordinal)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            #region Проверка параметров

            if (left == null)
            {
                throw new ArgumentNullException("left");
            }


            if (right == null)
            {
                throw new ArgumentNullException("right");
            }

            #endregion

            // Ищем начало позиции левой подстроки.
            int leftPosBegin = str.IndexOf(left, startIndex, comparsion);

            if (leftPosBegin == -1)
            {
                return string.Empty;
            }

            // Вычисляем конец позиции левой подстроки.
            int leftPosEnd = leftPosBegin + left.Length;

            // Ищем начало позиции правой подстроки.
            int rightPos = str.IndexOf(right, leftPosEnd, comparsion);

            if (rightPos == -1)
            {
                return string.Empty;
            }

            // Вычисляем длину найденной подстроки.
            int length = rightPos - leftPosEnd;

            return str.Substring(leftPosEnd, length);
        }

        /// <summary>
        /// Извлекает подстроку из строки. Подстрока ищется между двумя заданными строками.
        /// </summary>
        /// <param name="str">Строка, в которой будет поиск подстроки.</param>
        /// <param name="left">Строка, которая находится слева от искомой подстроки.</param>
        /// <param name="right">Строка, которая находится справа от искомой подстроки.</param>
        /// <param name="comparsion">Одно из значений перечисления, определяющее правила поиска.</param>
        /// <returns>Найденая подстрока, иначе пустая строка.</returns>
        /// <exception cref="System.ArgumentNullException">Значение параметра <paramref name="left"/> или <paramref name="right"/> равно <see langword="null"/>.</exception>
        /// <exception cref="System.ArgumentException">Значение параметра <paramref name="left"/> или <paramref name="right"/> является пустой строкой.</exception>
        public static string Substring(this string str, string left, string right,
            StringComparison comparsion = StringComparison.Ordinal)
        {
            return str.Substring(left, right, 0, comparsion);
        }

        /// <summary>
        /// Извлекает последнею подстроку из строки. Подстрока начинается с конца позиции подстроки <paramref name="left"/> и до конца строки. Поиск начинается с заданной позиции.
        /// </summary>
        /// <param name="str">Строка, в которой будет поиск последней подстроки.</param>
        /// <param name="left">Строка, которая находится слева от искомой подстроки.</param>
        /// <param name="startIndex">Позиция, с которой начинается поиск подстроки. Отсчёт от 0.</param>
        /// <param name="comparsion">Одно из значений перечисления, определяющее правила поиска.</param>
        /// <returns>Найденая подстрока, иначе пустая строка.</returns>
        /// <exception cref="System.ArgumentNullException">Значение параметра <paramref name="left"/> равно <see langword="null"/>.</exception>
        /// <exception cref="System.ArgumentException">Значение параметра <paramref name="left"/> является пустой строкой.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Значение параметра <paramref name="startIndex"/> меньше 0.
        /// -или-
        /// Значение параметра <paramref name="startIndex"/> равно или больше длины строки <paramref name="str"/>.
        /// </exception>
        public static string LastSubstring(this string str, string left,
            int startIndex, StringComparison comparsion = StringComparison.Ordinal)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            #region Проверка параметров

            if (left == null)
            {
                throw new ArgumentNullException("left");
            }

            #endregion

            // Ищем начало позиции левой подстроки.
            int leftPosBegin = str.LastIndexOf(left, startIndex, comparsion);

            if (leftPosBegin == -1)
            {
                return string.Empty;
            }

            // Вычисляем конец позиции левой подстроки.
            int leftPosEnd = leftPosBegin + left.Length;

            // Вычисляем длину найденной подстроки.
            int length = str.Length - leftPosEnd;

            return str.Substring(leftPosEnd, length);
        }

        /// <summary>
        /// Извлекает последнею подстроку из строки. Подстрока начинается с конца позиции подстроки <paramref name="left"/> и до конца строки.
        /// </summary>
        /// <param name="str">Строка, в которой будет поиск последней подстроки.</param>
        /// <param name="left">Строка, которая находится слева от искомой подстроки.</param>
        /// <param name="comparsion">Одно из значений перечисления, определяющее правила поиска.</param>
        /// <returns>Найденая подстрока, иначе пустая строка.</returns>
        /// <exception cref="System.ArgumentNullException">Значение параметра <paramref name="left"/> равно <see langword="null"/>.</exception>
        /// <exception cref="System.ArgumentException">Значение параметра <paramref name="left"/> является пустой строкой.</exception>
        public static string LastSubstring(this string str,
            string left, StringComparison comparsion = StringComparison.Ordinal)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            return LastSubstring(str, left, str.Length - 1, comparsion);
        }

        /// <summary>
        /// Извлекает последнею подстроку из строки. Подстрока ищется между двумя заданными строками, начиная с заданной позиции.
        /// </summary>
        /// <param name="str">Строка, в которой будет поиск последней подстроки.</param>
        /// <param name="left">Строка, которая находится слева от искомой подстроки.</param>
        /// <param name="right">Строка, которая находится справа от искомой подстроки.</param>
        /// <param name="startIndex">Позиция, с которой начинается поиск подстроки. Отсчёт от 0.</param>
        /// <param name="comparsion">Одно из значений перечисления, определяющее правила поиска.</param>
        /// <returns>Найденая подстрока, иначе пустая строка.</returns>
        /// <exception cref="System.ArgumentNullException">Значение параметра <paramref name="left"/> или <paramref name="right"/> равно <see langword="null"/>.</exception>
        /// <exception cref="System.ArgumentException">Значение параметра <paramref name="left"/> или <paramref name="right"/> является пустой строкой.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Значение параметра <paramref name="startIndex"/> меньше 0.
        /// -или-
        /// Значение параметра <paramref name="startIndex"/> равно или больше длины строки <paramref name="str"/>.
        /// </exception>
        public static string LastSubstring(this string str, string left, string right,
            int startIndex, StringComparison comparsion = StringComparison.Ordinal)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            #region Проверка параметров

            if (left == null)
            {
                throw new ArgumentNullException("left");
            }

            if (right == null)
            {
                throw new ArgumentNullException("right");
            }

            #endregion

            // Ищем начало позиции левой подстроки.
            int leftPosBegin = str.LastIndexOf(left, startIndex, comparsion);

            if (leftPosBegin == -1)
            {
                return string.Empty;
            }

            // Вычисляем конец позиции левой подстроки.
            int leftPosEnd = leftPosBegin + left.Length;

            // Ищем начало позиции правой подстроки.
            int rightPos = str.IndexOf(right, leftPosEnd, comparsion);

            if (rightPos == -1)
            {
                if (leftPosBegin == 0)
                {
                    return string.Empty;
                }
                else
                {
                    return LastSubstring(str, left, right, leftPosBegin - 1, comparsion);
                }
            }

            // Вычисляем длину найденной подстроки.
            int length = rightPos - leftPosEnd;

            return str.Substring(leftPosEnd, length);
        }

        /// <summary>
        /// Извлекает последнею подстроку из строки. Подстрока ищется между двумя заданными строками.
        /// </summary>
        /// <param name="str">Строка, в которой будет поиск последней подстроки.</param>
        /// <param name="left">Строка, которая находится слева от искомой подстроки.</param>
        /// <param name="right">Строка, которая находится справа от искомой подстроки.</param>
        /// <param name="comparsion">Одно из значений перечисления, определяющее правила поиска.</param>
        /// <returns>Найденая подстрока, иначе пустая строка.</returns>
        /// <exception cref="System.ArgumentNullException">Значение параметра <paramref name="left"/> или <paramref name="right"/> равно <see langword="null"/>.</exception>
        /// <exception cref="System.ArgumentException">Значение параметра <paramref name="left"/> или <paramref name="right"/> является пустой строкой.</exception>
        public static string LastSubstring(this string str, string left, string right,
            StringComparison comparsion = StringComparison.Ordinal)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            return str.LastSubstring(left, right, str.Length - 1, comparsion);
        }

        /// <summary>
        /// Извлекает подстроки из строки. Подстрока ищется между двумя заданными строками, начиная с заданной позиции.
        /// </summary>
        /// <param name="str">Строка, в которой будет поиск подстрок.</param>
        /// <param name="left">Строка, которая находится слева от искомой подстроки.</param>
        /// <param name="right">Строка, которая находится справа от искомой подстроки.</param>
        /// <param name="startIndex">Позиция, с которой начинается поиск подстрок. Отсчёт от 0.</param>
        /// <param name="comparsion">Одно из значений перечисления, определяющее правила поиска.</param>
        /// <returns>Найденые подстроки, иначе пустой массив строк.</returns>
        /// <exception cref="System.ArgumentNullException">Значение параметра <paramref name="left"/> или <paramref name="right"/> равно <see langword="null"/>.</exception>
        /// <exception cref="System.ArgumentException">Значение параметра <paramref name="left"/> или <paramref name="right"/> является пустой строкой.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Значение параметра <paramref name="startIndex"/> меньше 0.
        /// -или-
        /// Значение параметра <paramref name="startIndex"/> равно или больше длины строки <paramref name="str"/>.
        /// </exception>
        public static string[] Substrings(this string str, string left, string right,
            int startIndex, StringComparison comparsion = StringComparison.Ordinal)
        {
            if (string.IsNullOrEmpty(str))
            {
                return new string[0];
            }

            #region Проверка параметров

            if (left == null)
            {
                throw new ArgumentNullException("left");
            }

            if (right == null)
            {
                throw new ArgumentNullException("right");
            }

            #endregion

            int currentStartIndex = startIndex;
            List<string> strings = new List<string>();

            while (true)
            {
                // Ищем начало позиции левой подстроки.
                int leftPosBegin = str.IndexOf(left, currentStartIndex, comparsion);

                if (leftPosBegin == -1)
                {
                    break;
                }

                // Вычисляем конец позиции левой подстроки.
                int leftPosEnd = leftPosBegin + left.Length;

                // Ищем начало позиции правой строки.
                int rightPos = str.IndexOf(right, leftPosEnd, comparsion);

                if (rightPos == -1)
                {
                    break;
                }

                // Вычисляем длину найденной подстроки.
                int length = rightPos - leftPosEnd;

                strings.Add(str.Substring(leftPosEnd, length));

                // Вычисляем конец позиции правой подстроки.
                currentStartIndex = rightPos + right.Length;
            }

            return strings.ToArray();
        }

        /// <summary>
        /// Извлекает подстроки из строки. Подстрока ищется между двумя заданными строками.
        /// </summary>
        /// <param name="str">Строка, в которой будет поиск подстрок.</param>
        /// <param name="left">Строка, которая находится слева от искомой подстроки.</param>
        /// <param name="right">Строка, которая находится справа от искомой подстроки.</param>
        /// <param name="comparsion">Одно из значений перечисления, определяющее правила поиска.</param>
        /// <returns>Найденые подстроки, иначе пустой массив строк.</returns>
        /// <exception cref="System.ArgumentNullException">Значение параметра <paramref name="left"/> или <paramref name="right"/> равно <see langword="null"/>.</exception>
        /// <exception cref="System.ArgumentException">Значение параметра <paramref name="left"/> или <paramref name="right"/> является пустой строкой.</exception>
        public static string[] Substrings(this string str, string left, string right,
            StringComparison comparsion = StringComparison.Ordinal)
        {
            return str.Substrings(left, right, 0, comparsion);
        }

        #endregion
    }
}