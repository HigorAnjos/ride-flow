namespace Infrastructure.Storage.Services
{
    public class ImageStorageService : IImageStorageService
    {
        private string BaseDirectory => Path.Combine(AppContext.BaseDirectory, "Storage", "Images");

        public ImageStorageService()
        {
            Directory.CreateDirectory(BaseDirectory); 
        }

        public async Task<string> SaveImageAsync(string entityId, string base64Image, string imageType)
        {
            if (string.IsNullOrWhiteSpace(entityId))
                throw new ArgumentException("Entity ID cannot be null or empty.", nameof(entityId));

            if (string.IsNullOrWhiteSpace(base64Image))
                throw new ArgumentException("Base64 image cannot be null or empty.", nameof(base64Image));

            if (string.IsNullOrWhiteSpace(imageType))
                throw new ArgumentException("Image type cannot be null or empty.", nameof(imageType));

            // Define o diretório base da entidade
            Directory.CreateDirectory(BaseDirectory);

            // Remove o prefixo Base64 se existir
            const string base64Prefix = "data:image/";
            var extension = ".png"; // Valor padrão
            if (base64Image.StartsWith(base64Prefix))
            {
                var formatStart = base64Image.IndexOf('/') + 1;
                var formatEnd = base64Image.IndexOf(';');
                if (formatStart > 0 && formatEnd > formatStart)
                {
                    var format = base64Image.Substring(formatStart, formatEnd - formatStart);
                    extension = format switch
                    {
                        "jpeg" => ".jpg",
                        "png" => ".png",
                        "bmp" => ".bmp",
                        "gif" => ".gif",
                        _ => ".png" // Padrão caso o formato não seja reconhecido
                    };
                }

                base64Image = base64Image.Substring(base64Image.IndexOf(",") + 1); // Remove o cabeçalho
            }

            var fileName = $"{entityId}_{imageType}{extension}";
            var filePath = Path.Combine(BaseDirectory, fileName);

            // Remove arquivos anteriores associados à entidade
            DeleteExistingImages(entityId, imageType);

            // Salva a nova imagem
            var imageBytes = Convert.FromBase64String(base64Image);
            await File.WriteAllBytesAsync(filePath, imageBytes);

            return fileName; // Retorna o nome do arquivo com extensão
        }

        private void DeleteExistingImages(string entityId, string imageType)
        {
            var searchPattern = $"{entityId}_{imageType}.*"; // Busca todas as extensões possíveis
            var existingFiles = Directory.GetFiles(BaseDirectory, searchPattern);

            foreach (var file in existingFiles)
            {
                File.Delete(file);
            }
        }

        // Método utilitário para validar uma string Base64
        private bool IsBase64String(string base64)
        {
            Span<byte> buffer = new Span<byte>(new byte[base64.Length]);
            return Convert.TryFromBase64String(base64, buffer, out _);
        }

        public async Task<string> GetImageAsync(string entityId, string imageType)
        {
            if (string.IsNullOrWhiteSpace(entityId))
                throw new ArgumentException("Entity ID cannot be null or empty.", nameof(entityId));

            if (string.IsNullOrWhiteSpace(imageType))
                throw new ArgumentException("Image type cannot be null or empty.", nameof(imageType));

            var searchPattern = $"{entityId}_{imageType}.*"; // Busca qualquer extensão
            var files = Directory.GetFiles(BaseDirectory, searchPattern);

            if (files.Length == 0)
                return null; // Retorna null se nenhum arquivo for encontrado

            // Assume que só haverá um arquivo devido à lógica de exclusão
            var filePath = files[0];

            // Lê o arquivo e converte de volta para Base64
            var imageBytes = await File.ReadAllBytesAsync(filePath);
            var base64String = Convert.ToBase64String(imageBytes);

            // Adiciona o prefixo Base64 apropriado com base na extensão
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            var base64Prefix = extension switch
            {
                ".png" => "data:image/png;base64,",
                ".jpg" => "data:image/jpeg;base64,",
                ".jpeg" => "data:image/jpeg;base64,",
                ".bmp" => "data:image/bmp;base64,",
                ".gif" => "data:image/gif;base64,",
                _ => "data:image/png;base64," // Padrão
            };

            return $"{base64Prefix}{base64String}";
        }


        public async Task DeleteImageAsync(string entityId, string imageType)
        {
            if (string.IsNullOrWhiteSpace(entityId))
                throw new ArgumentException("Entity ID cannot be null or empty.", nameof(entityId));

            if (string.IsNullOrWhiteSpace(imageType))
                throw new ArgumentException("Image type cannot be null or empty.", nameof(imageType));

            var fileName = GenerateUniqueFileName(entityId, imageType);
            var filePath = Path.Combine(BaseDirectory, fileName);

            if (File.Exists(filePath))
            {
                await Task.Run(() => File.Delete(filePath));
            }
        }

        public string GenerateUniqueFileName(string entityId, string imageType)
        {
            // Define a extensão com base no tipo de imagem
            string extension = imageType.ToLower() switch
            {
                "licenseimage" => ".png", // Exemplo para imagens de licença
                "profilepicture" => ".jpg", // Caso queira suportar outro tipo
                _ => ".png" // Padrão
            };

            // Gera um nome único com a extensão
            return $"{entityId}_{imageType}{extension}";
        }

    }
}
