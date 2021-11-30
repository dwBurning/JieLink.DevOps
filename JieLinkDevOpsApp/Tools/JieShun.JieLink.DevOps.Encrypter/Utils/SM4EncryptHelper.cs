using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace JieShun.Udf.Core
{
    public static class SM4EncryptHelper
    {
        private const string LibPath = "Ciphertext";
        /// <summary>
        /// sm4加密算法
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <param name="len">输入长度</param>
        /// <param name="intPtr">intptr用于接收</param>
        /// <param name="outputlen">接收数据长度</param>
        /// <returns></returns>
        [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sm4_encrypt(string input, int len, out IntPtr intPtr, out int outputlen);

        /// <summary>
        /// sm4解密算法
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <param name="len">输入长度</param>
        /// <param name="intPtr">intptr用于接收</param>
        /// <param name="outputlen">接收数据长度</param>
        /// <returns></returns>
        [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sm4_decrypt(string input, int len, out IntPtr intPtr, out int outputlen);
        /// <summary>
        /// 释放内存  每次调用加密/解密后，都必须调用该函数释放内存
        /// </summary>
        /// <param name="intPtr"></param>
        /// <returns></returns>

        [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sm4_freebuf(out IntPtr intPtr);

        [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sm4_encrypt_binary(string input, int len, out IntPtr intPtr, out int outputlen);

        [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sm4_decrypt_binary(string input, int len, out IntPtr intPtr, out int outputlen);

        [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern int is_sm4_ciphertext(string input, int len);
    }
}