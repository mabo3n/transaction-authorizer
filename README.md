# Table of Contents

- [Description](#org660c44c)
- [Requirements](#org678f369)
- [Build](#orge90a2a4)
- [Run](#org0bfd422)
- [Further information](#org9d90e82)


<a id="org660c44c"></a>

# Description

A tiny command line application to authorize financial transactions on an account.

The application listens for json-formatted operations in the standard input, and outputs the results to the standard output. For a complete specification of the features and business rules, please refer to Nubank's **Code Challenge: Authorizer** document.


<a id="org678f369"></a>

# Requirements

[Docker](https://www.docker.com/)


<a id="orge90a2a4"></a>

# Build

```shell
docker build . -f Authorizer.Dockerfile -t authorizer:latest
```


<a id="org0bfd422"></a>

# Run

Example input:

```shell
cat > operations <<EOF
{"account": {"active-card": true, "available-limit": 100}}
{"transaction": {"merchant": "Burger King", "amount": 20, "time": "2019-02-13T10:00:00.000Z"}}
{"transaction": {"merchant": "Habbib's", "amount": 90, "time": "2019-02-13T11:00:00.000Z"}}
EOF
```

```shell
docker run -i --rm authorizer:latest < operations
```


<a id="org9d90e82"></a>

# Further information

See `RATIONALE.md`.
