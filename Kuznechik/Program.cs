using System.Text;

namespace Kuznechik
{
    internal class Program
    {
        public static byte[][] SplitStringTo128BitBlocks(string input)
        {
            const int blockSize = 16; // 128 бит = 16 байт
            byte[] utf8Bytes = Encoding.UTF8.GetBytes(input);
            int blockCount = (int)Math.Ceiling(utf8Bytes.Length / (double)blockSize);
            byte[][] blocks = new byte[blockCount][];

            for (int i = 0; i < blockCount; i++)
            {
                int offset = i * blockSize;
                int bytesToCopy = Math.Min(blockSize, utf8Bytes.Length - offset);
                blocks[i] = new byte[blockSize];

                // Копируем данные
                Array.Copy(utf8Bytes, offset, blocks[i], 0, bytesToCopy);

                // Заполняем остаток нулями (можно заменить на 0x20 для пробелов)
                for (int j = bytesToCopy; j < blockSize; j++)
                {
                    blocks[i][j] = 0x20;
                }
            }

            return blocks;
        }

        public static string Combine128BitBlocksToString(byte[][] blocks)
        {
            // Вычисляем общее количество байт
            int totalBytes = blocks.Length * 16;
            byte[] allBytes = new byte[totalBytes];

            // Копируем все байты из блоков в один массив
            for (int i = 0; i < blocks.Length; i++)
            {
                Array.Copy(blocks[i], 0, allBytes, i * 16, 16);
            }

            // Преобразуем байты в строку UTF-8, обрезая нули в конце
            string result = Encoding.UTF8.GetString(allBytes).TrimEnd('\0');
            return result;
        }
        static void Main(string[] args)
        {
            Crypt crypt = new Crypt();
            Console.WriteLine("Введите строку исходного текста...");
            string text = Console.ReadLine();
            Console.WriteLine();

            byte[][] blocks_to_encrypt = SplitStringTo128BitBlocks(text);
            byte[][] encrypted_blocks = new byte[blocks_to_encrypt.Length][];

            for (int i = 0; i < blocks_to_encrypt.Length; i++)
            {
                encrypted_blocks[i] = crypt.EncryptTextBlock(blocks_to_encrypt[i]);
            }

            Console.WriteLine("Зашифрованный текст:\n" + Combine128BitBlocksToString(encrypted_blocks));
            Console.WriteLine();

            byte[][] decrypted_blocks = new byte[encrypted_blocks.Length][];
            for (int i = 0; i < encrypted_blocks.Length; i++)
            {
                decrypted_blocks[i] = crypt.DecryptTextBlock(encrypted_blocks[i]);
            }

            Console.WriteLine("Расшифрованный текст:\n" + Combine128BitBlocksToString(decrypted_blocks));
        }
    }
}
