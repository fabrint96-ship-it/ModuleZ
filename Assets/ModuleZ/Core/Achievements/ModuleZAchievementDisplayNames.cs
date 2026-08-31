namespace ModuleZ.Core.Achievements
{
    public static class ModuleZAchievementDisplayNames
    {
        public static string Get(ModuleZAchievementId id)
        {
            switch (id)
            {
                case ModuleZAchievementId.FirstDuel:
                    return "Primer duelo";

                case ModuleZAchievementId.FirstVictory:
                    return "Primera victoria";

                case ModuleZAchievementId.ConquerorMadrid:
                    return "Conquistador de Madrid";

                case ModuleZAchievementId.ConquerorBarcelona:
                    return "Conquistador de Barcelona";

                case ModuleZAchievementId.ConquerorValencia:
                    return "Conquistador de Valencia";

                case ModuleZAchievementId.ConquerorAndalucia:
                    return "Conquistador de Andalucía";

                case ModuleZAchievementId.MainCampaignCompleted:
                    return "Campaña principal completada";

                case ModuleZAchievementId.TenVictories:
                    return "10 victorias";

                case ModuleZAchievementId.TwentyFiveVictories:
                    return "25 victorias";

                case ModuleZAchievementId.FiftyVictories:
                    return "50 victorias";

                case ModuleZAchievementId.FirstRematch:
                    return "Primer rematch";

                case ModuleZAchievementId.TenRematchesWon:
                    return "10 rematches ganados";

                case ModuleZAchievementId.TwentyFiveRematchesWon:
                    return "25 rematches ganados";

                case ModuleZAchievementId.AILevel50:
                    return "IA al 50%";

                case ModuleZAchievementId.AILevel100:
                    return "IA al 100%";

                case ModuleZAchievementId.VisitMadrid:
                    return "Visitar Madrid";

                case ModuleZAchievementId.VisitBarcelona:
                    return "Visitar Barcelona";

                case ModuleZAchievementId.VisitValencia:
                    return "Visitar Valencia";

                case ModuleZAchievementId.VisitAndalucia:
                    return "Visitar Andalucía";

                case ModuleZAchievementId.MetMiguel:
                    return "Conocí a Miguel";

                case ModuleZAchievementId.MetJordi:
                    return "Conocí a Jordi";

                case ModuleZAchievementId.MetVicent:
                    return "Conocí a Vicent";

                case ModuleZAchievementId.MetAntonio:
                    return "Conocí a Antonio";

                case ModuleZAchievementId.AllPersonalitiesDiscovered:
                    return "Toda España";

                case ModuleZAchievementId.PuzzleMaster:
                    return "Puzzle Master";

                case ModuleZAchievementId.ModuleZMaster:
                    return "Module Z Master";

                default:
                    return id.ToString();
            }
        }

        public static string GetDescription(ModuleZAchievementId id)
        {
            switch (id)
            {
                case ModuleZAchievementId.FirstDuel:
                    return "Participa en tu primer duelo.";

                case ModuleZAchievementId.FirstVictory:
                    return "Consigue tu primera victoria.";

                case ModuleZAchievementId.ConquerorMadrid:
                    return "Derrota al rival de Madrid.";

                case ModuleZAchievementId.ConquerorBarcelona:
                    return "Derrota al rival de Barcelona.";

                case ModuleZAchievementId.ConquerorValencia:
                    return "Derrota al rival de Valencia.";

                case ModuleZAchievementId.ConquerorAndalucia:
                    return "Derrota al rival de Andalucía.";

                case ModuleZAchievementId.MainCampaignCompleted:
                    return "Completa la campaña principal.";

                case ModuleZAchievementId.TenVictories:
                    return "Consigue 10 victorias.";

                case ModuleZAchievementId.TwentyFiveVictories:
                    return "Consigue 25 victorias.";

                case ModuleZAchievementId.FiftyVictories:
                    return "Consigue 50 victorias.";

                case ModuleZAchievementId.FirstRematch:
                    return "Juega tu primer rematch.";

                case ModuleZAchievementId.TenRematchesWon:
                    return "Gana 10 rematches.";

                case ModuleZAchievementId.TwentyFiveRematchesWon:
                    return "Gana 25 rematches.";

                case ModuleZAchievementId.AILevel50:
                    return "Alcanza una dificultad IA del 50%.";

                case ModuleZAchievementId.AILevel100:
                    return "Alcanza una dificultad IA del 100%.";

                case ModuleZAchievementId.VisitMadrid:
                    return "Visita Madrid.";

                case ModuleZAchievementId.VisitBarcelona:
                    return "Visita Barcelona.";

                case ModuleZAchievementId.VisitValencia:
                    return "Visita Valencia.";

                case ModuleZAchievementId.VisitAndalucia:
                    return "Visita Andalucía.";

                case ModuleZAchievementId.MetMiguel:
                    return "Descubre la personalidad de Miguel de Madrid.";

                case ModuleZAchievementId.MetJordi:
                    return "Descubre la personalidad de Jordi de Barcelona.";

                case ModuleZAchievementId.MetVicent:
                    return "Descubre la personalidad de Vicent de Valencia.";

                case ModuleZAchievementId.MetAntonio:
                    return "Descubre la personalidad de Antonio de Andalucía.";

                case ModuleZAchievementId.AllPersonalitiesDiscovered:
                    return "Descubre la personalidad de todos los rivales.";

                case ModuleZAchievementId.PuzzleMaster:
                    return "Derrota a todos los rivales.";

                case ModuleZAchievementId.ModuleZMaster:
                    return "Conviértete en Maestro de Module Z.";

                default:
                    return "";
            }
        }
    }
}