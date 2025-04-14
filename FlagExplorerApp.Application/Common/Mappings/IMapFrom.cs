
using AutoMapper;

namespace FlagExplorerApp.Application.Common.Mappings;

/// <summary>
/// This interface is used to map from a source type to a destination type.
/// </summary>
/// <typeparam name="T"></typeparam>
internal interface IMapFrom <T>
{
    void Mapping(Profile profile);
}
