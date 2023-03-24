using Lab8_CSharp.model;
using Lab8_CSharp.model.Validator;
using Lab8_CSharp.repository;

namespace Lab8_CSharp.service;

class JucatorActivService
{
    private IRepository<string, JucatorActiv> repository;

    public JucatorActivService(IRepository<string, JucatorActiv> repository)
    {
        this.repository = repository;
    }

    
    public List<JucatorActiv> findAllJucatoriActivi()
    {
        return repository.findAll().ToList();
    }

    
    public List<JucatorActiv> returneazaJucatoriActiviEchipa(string ech1Name,string ech2Name,string strDate)
    {
        //find all teams
        string fileName = "..\\..\\..\\data\\echipe.txt";
        EchipaValidator validator1 = new EchipaValidator();
        IRepository<string, Echipa> repository= new EchipeFileRepository(validator1,fileName);
        EchipaService serviceEchipa = new EchipaService(repository);
        List<Echipa> echipe = serviceEchipa.findAllEchipe();
        bool echipa1ok = false, echipa2ok = false;
        foreach (Echipa e in echipe)
        {
            if (e.Nume.Equals(ech1Name))
                echipa1ok = true;
            if (e.Nume.Equals(ech2Name))
                echipa2ok = true;
        }
        if((echipa1ok && echipa2ok)!=true)
            throw new Exception("Nu s-a gasit meciul cautat.Datele introduse sunt incorecte.");
        //create gameId
        string echipa1Id=echipe.Find(x => x.Nume.Equals(ech1Name)).ID;
        string echipa2Id=echipe.Find(x => x.Nume.Equals(ech2Name)).ID;
        string gameId;
        if(echipa1Id.CompareTo(echipa2Id)>0)
            gameId = echipa2Id + echipa1Id + "." + strDate;
        else
            gameId = echipa1Id + echipa2Id + "." + strDate;
        //get all activePlayers involved in the desired game
        List<JucatorActiv> jucatoriActivi =findAllJucatoriActivi();
        List<JucatorActiv> jucatoriMeci = new List<JucatorActiv>();
        foreach(JucatorActiv player in jucatoriActivi)
            if(player.idMeci.Equals(gameId))
                jucatoriMeci.Add(player);
        if (jucatoriMeci.Count == 0)
            throw new Exception("Nu s-a gasit meciul cautat.Datele introduse sunt incorecte.");
        //find all players
        string fileName2 = "..\\..\\..\\data\\jucatori.txt";
        JucatorValidator validator2 = new JucatorValidator();
        IRepository<string, Jucator> repository2= new JucatoriFileRepository(validator2,fileName2);
        JucatorService serviceJucatori = new JucatorService(repository2);
        List<Jucator> jucatori = serviceJucatori.findAllJucatori();
        //create a map with player ID and team ID
        var mapJucatorEchipa = new Dictionary<string, string>();
        foreach (Jucator j in jucatori)
            mapJucatorEchipa[j.ID] = j.Echipa.ID;
        //select all players in the desired team
        List<JucatorActiv> jucatoriEchipa = new List<JucatorActiv>();
        foreach(JucatorActiv j in jucatoriMeci)
            if(mapJucatorEchipa[j.ID].Equals(echipa1Id))
                jucatoriEchipa.Add(j);
        return jucatoriEchipa;
    }
}