namespace Infrastructure.Storage.Services
{
    public interface IImageStorageService
    {
        /// <summary>
        /// Salva uma imagem em Base64 no armazenamento usando um identificador genérico (como ID do usuário ou moto) e retorna o caminho do arquivo salvo.
        /// </summary>
        /// <param name="entityId">O identificador genérico para associar à imagem.</param>
        /// <param name="base64Image">A string da imagem em formato Base64.</param>
        /// <param name="imageType">O tipo de imagem (ex.: LicenseImage, PlateImage).</param>
        /// <returns>O caminho relativo do arquivo salvo.</returns>
        Task<string> SaveImageAsync(string entityId, string base64Image, string imageType);

        /// <summary>
        /// Busca uma imagem no armazenamento pelo identificador genérico e retorna a string Base64.
        /// </summary>
        /// <param name="entityId">O identificador genérico associado à imagem.</param>
        /// <param name="imageType">O tipo de imagem a ser buscada.</param>
        /// <returns>A string da imagem em formato Base64.</returns>
        Task<string> GetImageAsync(string entityId, string imageType);

        /// <summary>
        /// Exclui uma imagem no armazenamento pelo identificador genérico.
        /// </summary>
        /// <param name="entityId">O identificador genérico associado ao arquivo a ser excluído.</param>
        /// <param name="imageType">O tipo de imagem a ser excluída.</param>
        /// <returns>Uma tarefa representando a operação de exclusão.</returns>
        Task DeleteImageAsync(string entityId, string imageType);

        /// <summary>
        /// Gera um nome de arquivo único com base em um identificador genérico e o tipo de imagem.
        /// </summary>
        /// <param name="entityId">O identificador genérico para gerar o nome do arquivo.</param>
        /// <param name="imageType">O tipo de imagem a ser incluído no nome.</param>
        /// <returns>Um nome de arquivo único.</returns>
        string GenerateUniqueFileName(string entityId, string imageType);
    }
}
