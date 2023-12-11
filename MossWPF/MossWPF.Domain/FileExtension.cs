using System.Text.Json.Serialization;

namespace MossWPF.Domain
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum FileExtension
    {
        none
        , cpp
        , java
        , hs
        , f90
        , py
        , cs
        , js
        , json
        , ml
        , pas
        , ada
        , lisp
        , scm
        , asc
        , vhdl
        , perl
        , matlab
        , s
        , pl
        , sp
        , vb
        , mod
        , asm
        , h
        , f95
        , f03
        , hpp
        , cc
        , txt
            , c

    }

}
