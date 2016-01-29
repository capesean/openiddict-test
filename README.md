# openiddict-test
Example ASPNET5 project that issues &amp; validates JWT tokens. See http://capesean.co.za/blog/asp-net-5-jwt-tokens/ for full details.

This project uses OpenIddict to issue JWT tokens & refresh tokens. 

It uses ASP.NET Identity V3 and Entity Framework V7 for the user accounts & storage.

Simply run the project and it should seed the database (configure your connection string in the config file), and open the index.html page.

Click the *Get Token* button and OpenIddict should do it's magic and return a JWT token which will be displayed on the page.
