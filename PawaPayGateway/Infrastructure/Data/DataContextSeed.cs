using PawaPayGateway.Domain.Entities;

namespace PawaPayGateway.Infrastructure.Data
{
    public class DataContextSeed
    {
        /// <summary>
        /// Seed all data needed to run the Data services
        /// </summary>
        /// <param name="context"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static async Task SeedAsync(DataContext context, ILogger<DataContextSeed> logger)
        {
            //User account 
            if (!context.Gateways.Any())
            {
                context.Gateways.AddRange(GatewayInitializer());
                await context.SaveChangesAsync();

                //Log Information
                logger.LogInformation($"Gateways Database Seeded: {typeof(DataContext).Name}");
            }

            //Bank account type
            //if (!context.BankAccountTypes.Any())
            //{
            //    context.BankAccountTypes.AddRange(BankAccountTypeInitializer());
            //    await context.SaveChangesAsync();

            //    //Log Information
            //    logger.LogInformation($"BankAccountTypes Database Seeded: {typeof(DataContext).Name}");
            //}

            //Bank account 
            //if (!context.BankAccounts.Any())
            //{
            //	context.BankAccounts.AddRange(BankAccountInitializer());
            //	await context.SaveChangesAsync();

            //	//Log Information
            //	logger.LogInformation($"BankAccount Database Seeded: {typeof(DataContext).Name}");
            //}
        }


        #region Private

        /// <summary>
        /// Seed bank account types
        /// </summary>
        /// <returns></returns>
        //private static IEnumerable<BankAccountType> BankAccountTypeInitializer()
        //{
        //    return new[]
        //    {
        //        new BankAccountType()
        //        {
        //            Name = "SAVINGS",
        //            Description = "Savings account",
        //            CreatedOn = DateTime.Now,
        //        },
        //        new BankAccountType()
        //        {
        //            Name = "CURRENT",
        //            Description = "Current account",
        //            CreatedOn = DateTime.Now,
        //        }
        //    };
        //}

        /// <summary>
        /// Seed sample user account 
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<Gateway> GatewayInitializer()
        {
            return new[]
            {
                new Gateway()
                {
                    
                    CreatedOn= DateTime.Now,
                    LastModifiedOn = DateTime.Now,
                },
                new Gateway()
                {
                    
                    CreatedOn= DateTime.Now,
                    LastModifiedOn = DateTime.Now,
                }
            };
        }

        /// <summary>
        /// Seed sample bank account for users
        /// </summary>
        /// <returns></returns>
        //private static IEnumerable<BankAccount> BankAccountInitializer()
        //{
        //	//return new[]
        //	//{
        //	//	new BankAccount()
        //	//	{
        //	//		FirstName = "John",
        //	//		LastName = "Doe",
        //	//		PlaceOfBirth = "Magodo",
        //	//		DateOfBirth = new DateOnly(),
        //	//		Nationality = "Nigerian",
        //	//		Sex = "Male",
        //	//	},
        //	//	new BankAccount()
        //	//	{
        //	//		FirstName = "Mary",
        //	//		LastName = "Alex",
        //	//		PlaceOfBirth = "Ikeja",
        //	//		DateOfBirth = new DateOnly(),
        //	//		Nationality = "Nigerian",
        //	//		Sex = "Female",
        //	//	}
        //	//};
        //}

        #endregion
    }
}
