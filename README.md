## Mutacje

### 1. Register (Rejestracja)

Rejestruje nowego użytkownika w systemie.

**Parametry wejściowe:**
- `email` (wymagane, string): Adres email użytkownika (musi być poprawnym formatem email)
- `password` (wymagane, string): Hasło użytkownika (minimum 8 znaków)
- `firstName` (wymagane, string): Imię użytkownika (maksymalnie 50 znaków)
- `lastName` (wymagane, string): Nazwisko użytkownika (maksymalnie 50 znaków)

**Zwraca:**
- `token` (string): Token JWT do uwierzytelniania
- `user` (User): Obiekt użytkownika z danymi (id, email, firstName, lastName)

**Przykład użycia:**

```graphql
mutation {
  register(input: {
    email: "jan.kowalski@example.com"
    password: "SecurePassword123"
    firstName: "Jan"
    lastName: "Kowalski"
  }) {
    token
    user {
      id
      email
      firstName
      lastName
    }
  }
}
```

**Odpowiedź:**

```json
{
  "data": {
    "register": {
      "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
      "user": {
        "id": 1,
        "email": "jan.kowalski@example.com",
        "firstName": "Jan",
        "lastName": "Kowalski"
      }
    }
  }
}
```

---

### 2. Login (Logowanie)

Loguje istniejącego użytkownika do systemu.

**Parametry wejściowe:**
- `email` (wymagane, string): Adres email użytkownika
- `password` (wymagane, string): Hasło użytkownika

**Zwraca:**
- `token` (string): Token JWT do uwierzytelniania
- `user` (User): Obiekt użytkownika z danymi

**Przykład użycia:**

```graphql
mutation {
  login(input: {
    email: "jan.kowalski@example.com"
    password: "SecurePassword123"
  }) {
    token
    user {
      id
      email
      firstName
      lastName
    }
  }
}
```

**Odpowiedź:**

```json
{
  "data": {
    "login": {
      "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
      "user": {
        "id": 1,
        "email": "jan.kowalski@example.com",
        "firstName": "Jan",
        "lastName": "Kowalski"
      }
    }
  }
}
```

---

### 3. CreateTournament (Tworzenie turnieju)

Tworzy nowy turniej w systemie.

**Parametry wejściowe:**
- `name` (wymagane, string): Nazwa turnieju (maksymalnie 100 znaków)
- `startDate` (wymagane, DateTime): Data rozpoczęcia turnieju (musi być w przyszłości)

**Zwraca:**
- `id` (int): Identyfikator turnieju
- `name` (string): Nazwa turnieju
- `startDate` (DateTime): Data rozpoczęcia
- `status` (TournamentStatus): Status turnieju (Planned, Ongoing, Completed)
- `bracket` (Bracket?): Drabinka turniejowa (null, jeśli nie została jeszcze wygenerowana)
- `participants` (List<User>): Lista uczestników turnieju

**Przykład użycia:**

```graphql
mutation {
  createTournament(input: {
    name: "Turniej Piłki Nożnej 2024"
    startDate: "2024-12-25T10:00:00Z"
  }) {
    id
    name
    startDate
    status
    participants {
      id
      firstName
      lastName
    }
  }
}
```

**Odpowiedź:**

```json
{
  "data": {
    "createTournament": {
      "id": 1,
      "name": "Turniej Piłki Nożnej 2024",
      "startDate": "2024-12-25T10:00:00Z",
      "status": "Planned",
      "participants": []
    }
  }
}
```

---

### 4. AddParticipant (Dodawanie uczestnika)

Dodaje użytkownika do turnieju jako uczestnika.

**Parametry wejściowe:**
- `tournamentId` (wymagane, int): Identyfikator turnieju (musi być liczbą całkowitą dodatnią)
- `userId` (wymagane, int): Identyfikator użytkownika (musi być liczbą całkowitą dodatnią)

**Zwraca:**
- Obiekt `Tournament` z zaktualizowaną listą uczestników

**Przykład użycia:**

```graphql
mutation {
  addParticipant(tournamentId: 1, userId: 2) {
    id
    name
    participants {
      id
      firstName
      lastName
      email
    }
  }
}
```

**Odpowiedź:**

```json
{
  "data": {
    "addParticipant": {
      "id": 1,
      "name": "Turniej Piłki Nożnej 2024",
      "participants": [
        {
          "id": 2,
          "firstName": "Anna",
          "lastName": "Nowak",
          "email": "anna.nowak@example.com"
        }
      ]
    }
  }
}
```

---

### 5. StartTournament (Rozpoczęcie turnieju)

Zmienia status turnieju z "Planned" na "Ongoing".

**Parametry wejściowe:**
- `tournamentId` (wymagane, int): Identyfikator turnieju (musi być liczbą całkowitą dodatnią)

**Zwraca:**
- Obiekt `Tournament` ze zaktualizowanym statusem

**Przykład użycia:**

```graphql
mutation {
  startTournament(tournamentId: 1) {
    id
    name
    status
    startDate
  }
}
```

**Odpowiedź:**

```json
{
  "data": {
    "startTournament": {
      "id": 1,
      "name": "Turniej Piłki Nożnej 2024",
      "status": "Ongoing",
      "startDate": "2024-12-25T10:00:00Z"
    }
  }
}
```

---

### 6. GenerateBracket (Generowanie drabinki)

Generuje drabinkę turniejową dla określonego turnieju. Drabinka tworzy pary uczestników w systemie pucharowym.

**Parametry wejściowe:**
- `tournamentId` (wymagane, int): Identyfikator turnieju (musi być liczbą całkowitą dodatnią)

**Zwraca:**
- `id` (int): Identyfikator drabinki
- `tournamentId` (int): Identyfikator turnieju
- `matches` (List<Match>): Lista meczów w drabince

**Przykład użycia:**

```graphql
mutation {
  generateBracket(tournamentId: 1) {
    id
    tournamentId
    matches {
      id
      round
      player1 {
        id
        firstName
        lastName
      }
      player2 {
        id
        firstName
        lastName
      }
      winnerId
    }
  }
}
```

**Odpowiedź:**

```json
{
  "data": {
    "generateBracket": {
      "id": 1,
      "tournamentId": 1,
      "matches": [
        {
          "id": 1,
          "round": 1,
          "player1": {
            "id": 1,
            "firstName": "Jan",
            "lastName": "Kowalski"
          },
          "player2": {
            "id": 2,
            "firstName": "Anna",
            "lastName": "Nowak"
          },
          "winnerId": null
        }
      ]
    }
  }
}
```

---

### 7. PlayMatch (Rozegranie meczu)

Rejestruje wynik meczu, przypisując zwycięzcę.

**Parametry wejściowe:**
- `matchId` (wymagane, int): Identyfikator meczu (musi być liczbą całkowitą dodatnią)
- `winnerId` (wymagane, int): Identyfikator zwycięzcy (musi być liczbą całkowitą dodatnią)

**Zwraca:**
- `id` (int): Identyfikator meczu
- `round` (int): Runda meczu
- `player1Id` (int): Identyfikator pierwszego gracza
- `player2Id` (int): Identyfikator drugiego gracza
- `winnerId` (int?): Identyfikator zwycięzcy
- `bracketId` (int): Identyfikator drabinki
- `player1` (User): Obiekt pierwszego gracza
- `player2` (User): Obiekt drugiego gracza
- `winner` (User?): Obiekt zwycięzcy

**Przykład użycia:**

```graphql
mutation {
  playMatch(input: {
    matchId: 1
    winnerId: 2
  }) {
    id
    round
    player1 {
      id
      firstName
      lastName
    }
    player2 {
      id
      firstName
      lastName
    }
    winner {
      id
      firstName
      lastName
    }
    winnerId
  }
}
```

**Odpowiedź:**

```json
{
  "data": {
    "playMatch": {
      "id": 1,
      "round": 1,
      "player1": {
        "id": 1,
        "firstName": "Jan",
        "lastName": "Kowalski"
      },
      "player2": {
        "id": 2,
        "firstName": "Anna",
        "lastName": "Nowak"
      },
      "winner": {
        "id": 2,
        "firstName": "Anna",
        "lastName": "Nowak"
      },
      "winnerId": 2
    }
  }
}
```

---

## Statusy Turnieju

Turnieje mogą mieć następujące statusy:

- **Planned**: Turniej jest zaplanowany, ale jeszcze się nie rozpoczął
- **Ongoing**: Turniej jest w trakcie
- **Completed**: Turniej został zakończony

---

## Typowe przepływy pracy

### 1. Pełny cykl życia turnieju

```graphql
# 1. Rejestracja użytkowników
mutation {
  register(input: {
    email: "gracz1@example.com"
    password: "Haslo1234"
    firstName: "Gracz"
    lastName: "Pierwszy"
  }) {
    token
    user { id }
  }
}

# 2. Utworzenie turnieju
mutation {
  createTournament(input: {
    name: "Mój Turniej"
    startDate: "2024-12-25T10:00:00Z"
  }) {
    id
  }
}

# 3. Dodanie uczestników
mutation {
  addParticipant(tournamentId: 1, userId: 1) {
    id
  }
}

# 4. Rozpoczęcie turnieju
mutation {
  startTournament(tournamentId: 1) {
    id
    status
  }
}

# 5. Generowanie drabinki
mutation {
  generateBracket(tournamentId: 1) {
    id
    matches {
      id
      round
      player1 { id firstName lastName }
      player2 { id firstName lastName }
    }
  }
}

# 6. Rozegranie meczów
mutation {
  playMatch(input: {
    matchId: 1
    winnerId: 1
  }) {
    id
    winnerId
  }
}
```

---

## Uwagi

1. **Uwierzytelnianie**: Większość mutacji wymaga tokenu JWT w nagłówku `Authorization: Bearer <token>`
2. **Walidacja**: Wszystkie parametry wejściowe są walidowane. Błędy walidacji są zwracane w odpowiedzi GraphQL
3. **Baza danych**: System wykorzystuje bazę danych In-Memory, więc dane są resetowane po restarcie aplikacji
4. **Format dat**: Daty powinny być w formacie ISO 8601 (np. `2024-12-25T10:00:00Z`)

---

## Obsługa błędów

W przypadku błędów, GraphQL zwróci odpowiedź z sekcją `errors`:

```json
{
  "errors": [
    {
      "message": "Email jest wymagany",
      "extensions": {
        "code": "VALIDATION_ERROR"
      }
    }
  ]
}
```

Typowe błędy:
- **VALIDATION_ERROR**: Błąd walidacji danych wejściowych
- **UNAUTHORIZED**: Brak lub nieprawidłowy token uwierzytelniający
- **NOT_FOUND**: Nie znaleziono zasobu (np. turnieju, użytkownika, meczu)

