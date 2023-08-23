namespace TelegramBotPGFRC
{
    public class CystatinCBased:IEstimations
    {
        public decimal Calculate(PatientsAntropometrics antropometrics)
        {
           
            decimal result = 70.69m * Convert.ToDecimal(Math.Pow((double)antropometrics.SCystatinC, -0.931));
            return result = Math.Round(result, 2);
        }

    }
}
