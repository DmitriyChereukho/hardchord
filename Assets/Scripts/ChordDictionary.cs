using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChordDictionary
{
    // 12 нот в диезном виде
    private static readonly string[] Notes =
    {
        "C", "C#", "D", "D#", "E", "F",
        "F#", "G", "G#", "A", "A#", "B"
    };

    public static readonly Dictionary<string, AudioClip> MusicFileByNote = new()
    {
        ["C"] = Resources.Load<AudioClip>("Music/do"),
        ["D"] = Resources.Load<AudioClip>("Music/re"),
        ["E"] = Resources.Load<AudioClip>("Music/mi"),
        ["F"] = Resources.Load<AudioClip>("Music/fa"),
        ["G"] = Resources.Load<AudioClip>("Music/sol"),
        ["A"] = Resources.Load<AudioClip>("Music/lya"),
        ["B"] = Resources.Load<AudioClip>("Music/si")
    };

    // Словарь, где ключ – это множество (Set) из 3 нот, а значение – название аккорда
    public static readonly Dictionary<HashSet<string>, string> ChordMap
        = new(new HashSetComparer());

    // Инициализация (можно вызвать из конструктора или статического конструктора)
    static ChordDictionary()
    {
        // Генерируем все Major, Minor, Diminished, Augmented трезвучия
        for (var i = 0; i < Notes.Length; i++)
        {
            var root = Notes[i];

            // Мажор (0–4–7)
            var majorThird = Notes[(i + 4) % 12];
            var perfectFifth = Notes[(i + 7) % 12];
            AddChord(new[] { root, majorThird, perfectFifth }, root + " Major");

            // Минор (0–3–7)
            var minorThird = Notes[(i + 3) % 12];
            perfectFifth = Notes[(i + 7) % 12];
            AddChord(new[] { root, minorThird, perfectFifth }, root + " Minor");

            // Уменьшенное (0–3–6)
            var diminishedFifth = Notes[(i + 6) % 12];
            AddChord(new[] { root, minorThird, diminishedFifth }, root + " Diminished");

            // Увеличенное (0–4–8)
            var augmentedFifth = Notes[(i + 8) % 12];
            AddChord(new[] { root, majorThird, augmentedFifth }, root + " Augmented");
        }
    }

    private static void AddChord(string[] notes, string chordName)
    {
        // Превратим массив нот в множество
        var noteSet = new HashSet<string>(notes);
        // Добавим в словарь, если ещё нет
        if (!ChordMap.ContainsKey(noteSet))
        {
            ChordMap[noteSet] = chordName;
        }
    }

    // Метод проверки, является ли трио нот трезвучием из словаря
    public static bool TryGetChordName(string note1, string note2, string note3, out string chordName)
    {
        // Приводим к множеству
        var set = new HashSet<string> { note1, note2, note3 };
        // Ищем в словаре
        return ChordMap.TryGetValue(set, out chordName);
    }

    // Класс для корректного сравнения HashSet'ов в Dictionary
    private class HashSetComparer : IEqualityComparer<HashSet<string>>
    {
        public bool Equals(HashSet<string> x, HashSet<string> y)
        {
            return x != null && y != null && x.SetEquals(y);
        }

        public int GetHashCode(HashSet<string> obj)
        {
            // Чтобы хеш не зависел от порядка, воспользуемся суммой хешей
            // или любым иным способом, который не чувствителен к порядку.
            return obj.Aggregate(0, (current, note) => current ^ note.GetHashCode());
        }
    }
}