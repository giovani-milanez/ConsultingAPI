# ConsultingAPI
API para consultores realizar atendimento online

scaffold models:
dotnet ef dbcontext scaffold "Server=localhost;Database=consultingapi;Uid=root;Pwd=mysql" Pomelo.EntityFrameworkCore.MySql -o Model -f -c DatabaseContext --context-dir Model/Context --data-annotations -f --project Database