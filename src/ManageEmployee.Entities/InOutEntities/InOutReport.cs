using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities.InOutEntities;

public class InOutReport
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    [MaxLength(36)]
    public string? Day1 { get; set; }
    [MaxLength(36)]
    public string? Day2 { get; set; }
    [MaxLength(36)]
    public string? Day3 { get; set; }
    [MaxLength(36)]
    public string? Day4 { get; set; }
    [MaxLength(36)]
    public string? Day5 { get; set; }
    [MaxLength(36)]
    public string? Day6 { get; set; }
    [MaxLength(36)]
    public string? Day7 { get; set; }
    [MaxLength(36)]
    public string? Day8 { get; set; }
    [MaxLength(36)]
    public string? Day9 { get; set; }
    [MaxLength(36)]
    public string? Day10 { get; set; }
    [MaxLength(36)]
    public string? Day11 { get; set; }
    [MaxLength(36)]
    public string? Day12 { get; set; }
    [MaxLength(36)]
    public string? Day13 { get; set; }
    [MaxLength(36)]
    public string? Day14 { get; set; }
    [MaxLength(36)]
    public string? Day15 { get; set; }
    [MaxLength(36)]
    public string? Day16 { get; set; }
    [MaxLength(36)]
    public string? Day17 { get; set; }
    [MaxLength(36)]
    public string? Day18 { get; set; }
    [MaxLength(36)]
    public string? Day19 { get; set; }
    [MaxLength(36)]
    public string? Day20 { get; set; }
    [MaxLength(36)]
    public string? Day21 { get; set; }
    [MaxLength(36)]
    public string? Day22 { get; set; }
    [MaxLength(36)]
    public string? Day23 { get; set; }
    [MaxLength(36)]
    public string? Day24 { get; set; }
    [MaxLength(36)]
    public string? Day25 { get; set; }
    [MaxLength(36)]
    public string? Day26 { get; set; }
    [MaxLength(36)]
    public string? Day27 { get; set; }
    [MaxLength(36)]
    public string? Day28 { get; set; }
    [MaxLength(36)]
    public string? Day29 { get; set; }
    [MaxLength(36)]
    public string? Day30 { get; set; }
    [MaxLength(36)]
    public string? Day31 { get; set; }
}