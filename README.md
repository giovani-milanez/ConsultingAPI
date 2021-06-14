# ConsultingAPI
API para consultores realizar atendimento online

scaffold models:
dotnet ef dbcontext scaffold "Server=localhost;Database=consultingapi;Uid=root;Pwd=mysql" Pomelo.EntityFrameworkCore.MySql -o Models -c DatabaseContext --data-annotations -f --project Database

to replace: add -f