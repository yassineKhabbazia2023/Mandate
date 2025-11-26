namespace Pulse.Back.Accounting.Mandate.Application.Exceptions
{
    public static class Errors
    {
        public static readonly string NotAuthorizedBankCode = "MAN001";
        public static readonly string NotAuthorizedBankCodeMessage = "La banque avec le code '{0}' ne peut pas faire l'objet d'une création sur la plateforme RYDGE. Merci de contacter les équipes FR-FM Mes Mandats : fr-mesmandats@rydge.fr.";

        public static readonly string NotFoundRoleCode = "ACC004";
        public static readonly string NotFoundRoleMessage = "Le contact avec l'identifiant {0} n'a aucun role sur l'account {1}";

    }
}
