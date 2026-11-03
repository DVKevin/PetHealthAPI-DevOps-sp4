namespace PetHealthAPI.Dominio.Entidades;

/// <summary>
/// Pet cadastrado na plataforma, sempre vinculado a um tutor.
/// </summary>
public class Pet
{
    public const int IdadeMaxima = 30;
    public const decimal PesoMaximo = 200m;

    public int Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string Especie { get; private set; } = string.Empty;
    public string? Raca { get; private set; }
    public int? Idade { get; private set; }
    public decimal? Peso { get; private set; }
    public string? Sexo { get; private set; }
    public bool Castrado { get; private set; }
    public string? Alergias { get; private set; }
    public DateTime? DataNascimento { get; private set; }
    public DateTime DataCadastro { get; private set; }

    public int TutorId { get; private set; }
    public Tutor? Tutor { get; private set; }

    public ICollection<Vacina> Vacinas { get; private set; } = new List<Vacina>();
    public ICollection<Consulta> Consultas { get; private set; } = new List<Consulta>();
    public ICollection<Medicamento> Medicamentos { get; private set; } = new List<Medicamento>();

    // Construtor exigido pelo EF Core
    protected Pet() { }

    public Pet(string nome, string especie, string? raca, int? idade, decimal? peso, string? sexo,
               bool castrado, string? alergias, DateTime? dataNascimento, int tutorId)
    {
        Validar(nome, especie, idade, peso, dataNascimento, tutorId);

        Nome = nome.Trim();
        Especie = especie.Trim();
        Raca = raca?.Trim();
        Idade = idade;
        Peso = peso;
        Sexo = sexo?.Trim();
        Castrado = castrado;
        Alergias = alergias?.Trim();
        DataNascimento = dataNascimento;
        TutorId = tutorId;
        DataCadastro = DateTime.Now;
    }

    public void Atualizar(string nome, string especie, string? raca, int? idade, decimal? peso, string? sexo,
                          bool castrado, string? alergias, DateTime? dataNascimento, int tutorId)
    {
        Validar(nome, especie, idade, peso, dataNascimento, tutorId);

        Nome = nome.Trim();
        Especie = especie.Trim();
        Raca = raca?.Trim();
        Idade = idade;
        Peso = peso;
        Sexo = sexo?.Trim();
        Castrado = castrado;
        Alergias = alergias?.Trim();
        DataNascimento = dataNascimento;
        TutorId = tutorId;
    }

    private static void Validar(string nome, string especie, int? idade, decimal? peso,
                                DateTime? dataNascimento, int tutorId)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("O nome do pet é obrigatório.", nameof(nome));

        if (nome.Trim().Length > 100)
            throw new ArgumentException("O nome do pet deve ter no máximo 100 caracteres.", nameof(nome));

        if (string.IsNullOrWhiteSpace(especie))
            throw new ArgumentException("A espécie do pet é obrigatória.", nameof(especie));

        if (idade < 0)
            throw new ArgumentException("A idade do pet não pode ser negativa.", nameof(idade));

        if (idade > IdadeMaxima)
            throw new ArgumentException("A idade do pet deve ser de no máximo 30 anos.", nameof(idade));

        if (peso <= 0)
            throw new ArgumentException("O peso do pet deve ser maior que zero.", nameof(peso));

        if (peso > PesoMaximo)
            throw new ArgumentException("O peso do pet deve ser de no máximo 200 kg.", nameof(peso));

        if (dataNascimento > DateTime.Now)
            throw new ArgumentException("A data de nascimento do pet não pode ser futura.", nameof(dataNascimento));

        if (tutorId <= 0)
            throw new ArgumentException("O tutor do pet deve ser informado.", nameof(tutorId));
    }
}
