﻿using System;
using System.Collections.Generic;
using System.Text;

namespace xNet.Text
{
    /// <summary>
    /// Представляет статический класс, предназначенный для помощи в работе со строками.
    /// </summary>
    public static class StringHelper
    {
        #region Статические методы (открытые)

        /// <summary>
        /// Инвертирует все симолы в строке.
        /// </summary>
        /// <param name="str">Строка, в которой будут инвертированы все символы.</param>
        /// <returns>Строка с инвертированными символами.</returns>
        public static string Reverse(this string str)
        {
            if (string.IsNullOrEmpty(str) || str.Length == 1)
            {
                return string.Empty;
            }

            var strBuilder = new StringBuilder();

            for (var i = str.Length - 1; i >= 0; --i)
            {
                strBuilder.Append(str[i]);
            }

            return strBuilder.ToString();
        }

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

            if (left.Length == 0)
            {
                throw ExceptionHelper.EmptyString("left");
            }

            if (startIndex < 0)
            {
                throw ExceptionHelper.CanNotBeLess("startIndex", 0);
            }

            if (startIndex >= str.Length)
            {
                throw new ArgumentOutOfRangeException("startIndex",
                    Resources.ArgumentOutOfRangeException_StringHelper_MoreLengthString);
            }

            #endregion

            // Ищем начало позиции левой подстроки.
            var leftPosBegin = str.IndexOf(left, startIndex, comparsion);

            if (leftPosBegin == -1)
            {
                return string.Empty;
            }

            // Вычисляем конец позиции левой подстроки.
            var leftPosEnd = leftPosBegin + left.Length;

            // Вычисляем длину найденной подстроки.
            var length = str.Length - leftPosEnd;

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

            if (left.Length == 0)
            {
                throw ExceptionHelper.EmptyString("left");
            }

            if (right == null)
            {
                throw new ArgumentNullException("right");
            }

            if (right.Length == 0)
            {
                throw ExceptionHelper.EmptyString("right");
            }

            if (startIndex < 0)
            {
                throw ExceptionHelper.CanNotBeLess("startIndex", 0);
            }

            if (startIndex >= str.Length)
            {
                throw new ArgumentOutOfRangeException("startIndex",
                    Resources.ArgumentOutOfRangeException_StringHelper_MoreLengthString);
            }

            #endregion

            // Ищем начало позиции левой подстроки.
            var leftPosBegin = str.IndexOf(left, startIndex, comparsion);

            if (leftPosBegin == -1)
            {
                return string.Empty;
            }

            // Вычисляем конец позиции левой подстроки.
            var leftPosEnd = leftPosBegin + left.Length;

            // Ищем начало позиции правой подстроки.
            var rightPos = str.IndexOf(right, leftPosEnd, comparsion);

            if (rightPos == -1)
            {
                return string.Empty;
            }

            // Вычисляем длину найденной подстроки.
            var length = rightPos - leftPosEnd;

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
            StringComparison comparsion = StringComparison.Ordinal) => str.Substring(left, right, 0, comparsion);

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

            if (left.Length == 0)
            {
                throw ExceptionHelper.EmptyString("left");
            }

            if (startIndex < 0)
            {
                throw ExceptionHelper.CanNotBeLess("startIndex", 0);
            }

            if (startIndex >= str.Length)
            {
                throw new ArgumentOutOfRangeException("startIndex",
                    Resources.ArgumentOutOfRangeException_StringHelper_MoreLengthString);
            }

            #endregion

            // Ищем начало позиции левой подстроки.
            var leftPosBegin = str.LastIndexOf(left, startIndex, comparsion);

            if (leftPosBegin == -1)
            {
                return string.Empty;
            }

            // Вычисляем конец позиции левой подстроки.
            var leftPosEnd = leftPosBegin + left.Length;

            // Вычисляем длину найденной подстроки.
            var length = str.Length - leftPosEnd;

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
            string left, StringComparison comparsion = StringComparison.Ordinal) => string.IsNullOrEmpty(str) ? string.Empty : LastSubstring(str, left, str.Length - 1, comparsion);

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
        public static string LastSubstring(this string str, string left, string right, int startIndex, StringComparison comparsion = StringComparison.Ordinal)
        {
            while (true)
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

                if (left.Length == 0)
                {
                    throw ExceptionHelper.EmptyString("left");
                }

                if (right == null)
                {
                    throw new ArgumentNullException("right");
                }

                if (right.Length == 0)
                {
                    throw ExceptionHelper.EmptyString("right");
                }

                if (startIndex < 0)
                {
                    throw ExceptionHelper.CanNotBeLess("startIndex", 0);
                }

                if (startIndex >= str.Length)
                {
                    throw new ArgumentOutOfRangeException("startIndex", Resources.ArgumentOutOfRangeException_StringHelper_MoreLengthString);
                }

                #endregion

                // Ищем начало позиции левой подстроки.
                var leftPosBegin = str.LastIndexOf(left, startIndex, comparsion);

                if (leftPosBegin == -1)
                {
                    return string.Empty;
                }

                // Вычисляем конец позиции левой подстроки.
                var leftPosEnd = leftPosBegin + left.Length;

                // Ищем начало позиции правой подстроки.
                var rightPos = str.IndexOf(right, leftPosEnd, comparsion);

                if (rightPos == -1)
                {
                    if (leftPosBegin == 0) return string.Empty;
                    startIndex = leftPosBegin - 1;
                    continue;
                }

                // Вычисляем длину найденной подстроки.
                var length = rightPos - leftPosEnd;

                return str.Substring(leftPosEnd, length);
            }
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
            StringComparison comparsion = StringComparison.Ordinal) => string.IsNullOrEmpty(str) ? string.Empty : str.LastSubstring(left, right, str.Length - 1, comparsion);

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

            if (left.Length == 0)
            {
                throw ExceptionHelper.EmptyString("left");
            }

            if (right == null)
            {
                throw new ArgumentNullException("right");
            }

            if (right.Length == 0)
            {
                throw ExceptionHelper.EmptyString("right");
            }

            if (startIndex < 0)
            {
                throw ExceptionHelper.CanNotBeLess("startIndex", 0);
            }

            if (startIndex >= str.Length)
            {
                throw new ArgumentOutOfRangeException("startIndex",
                    Resources.ArgumentOutOfRangeException_StringHelper_MoreLengthString);
            }

            #endregion

            var currentStartIndex = startIndex;
            var strings = new List<string>();

            while (true)
            {
                // Ищем начало позиции левой подстроки.
                var leftPosBegin = str.IndexOf(left, currentStartIndex, comparsion);

                if (leftPosBegin == -1)
                {
                    break;
                }

                // Вычисляем конец позиции левой подстроки.
                var leftPosEnd = leftPosBegin + left.Length;

                // Ищем начало позиции правой строки.
                var rightPos = str.IndexOf(right, leftPosEnd, comparsion);

                if (rightPos == -1)
                {
                    break;
                }

                // Вычисляем длину найденной подстроки.
                var length = rightPos - leftPosEnd;

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
            StringComparison comparsion = StringComparison.Ordinal) => str.Substrings(left, right, 0, comparsion);

        #endregion
    }
}