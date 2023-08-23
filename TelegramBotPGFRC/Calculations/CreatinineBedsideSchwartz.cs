

namespace  TelegramBotPGFRC
{
    public class CreatinineBedsideSchwartz : IEstimations
    {
       
        public decimal Calculate(PatientsAntropometrics antropometrics)
        {
            decimal result = 0.413m * antropometrics.Height / (antropometrics.SCreatinine/88.4m);
            return Math.Round(result, 2);
        }
    }
}
