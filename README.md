### Relacionamentos

* **Uma Empresa pode ter várias Candidaturas**, mas cada Candidatura pertence a uma única Empresa.
* **Um Currículo pode ser usado em várias Candidaturas**, permitindo registrar qual versão do currículo foi enviada para cada empresa.
* **Uma Candidatura pode ter vários registros de Histórico de Contato**, como e-mails, ligações, entrevistas e outros contatos.
* **Cada Histórico de Contato pertence a uma única Candidatura**.

### Resumo

```text
Empresa
   │
   │ 1:N
   ▼
Candidatura
   ▲
   │ N:1
   │
Currículo

Candidatura
   │
   │ 1:N
   ▼
Histórico de Contato
```

Dessa forma, o sistema mantém o histórico das candidaturas e permite saber qual currículo foi utilizado em cada candidatura.

---

## aplicar migration

```bash
dotnet ef migrations add V1 \
  --project TalentSendSync.Infrastructure \
  --startup-project TalentSendSync.API \
  --output-dir Migrations
```

## atualiza banco
```bash
dotnet ef database update \
  --project TalentSendSync.Infrastructure \
  --startup-project TalentSendSync.API
```