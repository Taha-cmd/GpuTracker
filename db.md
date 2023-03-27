For our service, we selected SQLite as database of our choice because of multiple reasons:
One of the reason was suggested to be because of the dev/prod parity of the twelve-factor app.
We want to stay as close to production as we are developing it, so using SQLite was the go-to choice.
SQLite is easy to use and easy to learn, making it pretty straight forward for our app.
Although it is a lightweight db system, it can handle a large amount of data, which would assure the stabilitty and scalability of our service.
It is compatible with all tools that we are using: Kafka, Kubernetes and Docker using C#.
Low resource req.: SQLite uses only limited CPU and Memory, making it viable even on slower Computers.
ACID Compliance: It ensures data consistency and integrity, ensuring accurate and up-to-date data for GPU Prices.