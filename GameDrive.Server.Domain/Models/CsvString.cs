using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace GameDrive.Server.Domain.Models;

[Owned]
public class CsvString
{
    private ICollection<string>? _cachedList;
    private string _value = string.Empty;
    public string Delimiter { get; set; } = ",";

    public string Value
    {
        get => _value;
        set
        {
            _cachedList = null;
            _value = value;
        }
    }

    [NotMapped] public ICollection<string> List => GetValueAsList();

    public static implicit operator CsvString(Collection<string> stringCollection) => new CsvString(stringCollection);
    public static implicit operator CsvString(List<string> stringCollection) => new CsvString(stringCollection);

    public CsvString()
    {
    }

    public CsvString(string delimiter)
    {
        Delimiter = delimiter;
    }

    public CsvString(IEnumerable<string> enumerable, string? delimiter = null)
    {
        if (!string.IsNullOrWhiteSpace(delimiter))
            Delimiter = delimiter;

        SetListAsValue(enumerable);
    }


    public void SetListAsValue(IEnumerable<string> enumerable)
    {
        var list = enumerable.ToList();
        var sb = new StringBuilder();
        for (var i = 0; i < list.Count; i++)
        {
            sb.Append(list.ElementAt(i));

            if (i < list.Count - 1)
                sb.Append(Delimiter);
        }

        _value = sb.ToString();
    }

    private ICollection<string> GetValueAsList()
    {
        if (_cachedList is not null)
            return _cachedList;

        _cachedList = Value.Split(Delimiter);
        return _cachedList;
    }
}