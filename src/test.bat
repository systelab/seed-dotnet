dotnet test --logger:trx --results-directory ../TestResults
allure serve ./TestResults

