using Application.Interactors.Events;
using Application.UseCase;
using Domain.Plans;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Application.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class DependencyInjection
    {
        public static IServiceCollection AddAplication(this IServiceCollection services)
            => services.AddInteractors()
                       .AddUseCases();

        private static IServiceCollection AddUseCases(this IServiceCollection services)
        {
            // DeliveryPerson
            services.AddScoped<ICreateDeliveryPersonUseCase, CreateDeliveryPersonUseCase>();
            services.AddScoped<IUploadLicenseImageUseCase, UploadLicenseImageUseCase>();

            // Motorcycle
            services.AddScoped<IGetMotorcycleByIdUseCase, GetMotorcycleByIdUseCase>();
            services.AddScoped<IGetMotorcyclesUseCase, GetMotorcyclesUseCase>();
            services.AddScoped<ICreateMotorcycleUseCase, CreateMotorcycleUseCase>();
            services.AddScoped<IRentMotorcycleUseCase, RentMotorcycleUseCase>();
            services.AddScoped<IUpdateMotorcycleLicensePlateUseCase, UpdateMotorcycleLicensePlateUseCase>();
            services.AddScoped<IDeleteMotorcycleUseCase, DeleteMotorcycleUseCase>();
            services.AddScoped<IHandleNewMotorcycleCreatedUseCase, HandleNewMotorcycleCreatedUseCase>();

            // Rental
            services.AddScoped<IGetRentalByIdUseCase, GetRentalByIdUseCase>();
            services.AddScoped<IUpdateRentalReturnDateUseCase, UpdateRentalReturnDateUseCase>();
            services.AddScoped<IRentalPlanFactory, RentalPlanFactory>();

            return services;
        }

        private static IServiceCollection AddInteractors(this IServiceCollection services)
        {
            services.AddScoped<INewMotorcycleCreatedEvenInteractor, NewMotorcycleCreatedEvenInteractor>();
            return services;
        }
    }
}
