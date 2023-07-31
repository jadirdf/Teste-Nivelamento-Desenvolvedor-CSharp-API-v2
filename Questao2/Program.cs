using System;
using System.Globalization;


using Questao2;

public class Program
{
    public static void Main()
    {

        string teamName = "Paris Saint-Germain";
        int year = 2013;

        int totalGoals = getTotalScoredGoals(teamName, year);

        Console.WriteLine("Team "+ teamName +" scored "+ totalGoals.ToString() + " goals in "+ year);


        teamName = "Chelsea";
        year = 2014;
        totalGoals = getTotalScoredGoals(teamName, year);

        Console.WriteLine("Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year);

        // Output expected:
        // Team Paris Saint - Germain scored 109 goals in 2013
        // Team Chelsea scored 92 goals in 2014
    }

    public static int getTotalScoredGoals(string teamName, int year)
    {
        PartidaServices partidaServices = new PartidaServices();

        int scored = 0;

        #region recupera a quantidade de gols marcados na primeira página de resultados do Time 1

        var retorno = partidaServices.Consulta(year, teamName, string.Empty);

        foreach (var partida in retorno.Result.Data)
        {
            scored += int.Parse(partida.Team1goals);
        }

        #endregion

        #region recupera a quantidade de gols marcados das outras página de resultados do Time 1 se houver

        int totalPages = retorno.Result.Total_pages;
        for (int page = 2; page <= totalPages; page++)
        {
            retorno = partidaServices.Consulta(year, teamName, string.Empty, page);

            foreach (var partida in retorno.Result.Data)
            {
                scored += int.Parse(partida.Team1goals);
            }

        }

        #endregion


        #region recupera a quantidade de gols marcados na primeira página de resultados do Time 2

        retorno = partidaServices.Consulta(year, string.Empty, teamName);

        foreach (var partida in retorno.Result.Data)
        {
            scored += int.Parse(partida.Team2goals);
        }

        #endregion

        #region recupera a quantidade de gols marcados das outras página de resultados do Time 2 se houver

        totalPages = retorno.Result.Total_pages;
        for (int page = 2; page <= totalPages; page++)
        {
            retorno = partidaServices.Consulta(year, string.Empty, teamName, page);

            foreach (var partida in retorno.Result.Data)
            {
                scored += int.Parse(partida.Team2goals);
            }

        }

        #endregion

        return scored;
    }

}