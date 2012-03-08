

using System;

namespace DataGenerator
{
public class State
{
  	public int Id {get;set;}

	public int CountryId {get;set;}

	public string Name {get;set;}

	public string Abbrev {get;set;}

}
public class Country
{
  	public int Id {get;set;}

	public string ISO {get;set;}

	public string Name {get;set;}

	public string PrintableName {get;set;}

	public string ISO3 {get;set;}

	public short? NumCode {get;set;}

	public bool HasState {get;set;}

}
}