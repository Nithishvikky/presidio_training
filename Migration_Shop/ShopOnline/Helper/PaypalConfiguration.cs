using PayPal.Api;

namespace ShopOnline.Helper
{
    public static class PaypalConfiguration
    {
        public static readonly string ClientId = "ARUwVw3aOSnyzOuxOpnjHjbS9dN9VNcSr1fR3aT5Rm5V_756zspYHvuXyZNADcjK0p16OQ7IOlc8SI9W";
        public static readonly string ClientSecret = "EBb3mpbOGjYzkiLHljgDY6y7taeh6fm_D2hlo_vFuQV1SEx2sBTeIi4lqpB48UCwSE-b1u6-TZNGjAeD";

        public static APIContext GetAPIContext()
        {
            var config = new Dictionary<string, string>
            {
                { "mode", "sandbox" },
                { "clientId", ClientId },
                { "clientSecret", ClientSecret }
            };

            var accessToken = new OAuthTokenCredential(ClientId, ClientSecret, config).GetAccessToken();
            var apiContext = new APIContext(accessToken)
            {
                Config = config
            };

            return apiContext;
        }
    }
}