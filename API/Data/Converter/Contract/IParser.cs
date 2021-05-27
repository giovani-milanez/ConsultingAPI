using System.Collections.Generic;

namespace API.Data.Converter.Contract
{
    // where O -> Origem
    //       D -> Destino
    public interface IParser<O, D>
    {
        D Parse(O origin);
        List<D> Parse(List<O> origin);
    }
}
