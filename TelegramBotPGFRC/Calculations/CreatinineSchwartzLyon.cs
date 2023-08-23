namespace TelegramBotPGFRC
{
    public class CreatinineSchwartzLyon:IEstimations
    {
        public decimal Calculate(PatientsAntropometrics antropometrics)
        {
           
            decimal Q = antropometrics.Gender == Gender.Male & antropometrics.Age > 13 ? 0.413m : 0.368m;

            decimal result = Q * antropometrics.Height / antropometrics.SCreatinine;
            return result = Math.Round(result, 2);
           

        }
    }
}
