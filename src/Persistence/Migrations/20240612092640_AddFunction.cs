using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFunction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE FUNCTION dbo.Levenshtein(@s1 nvarchar(max), @s2 nvarchar(max))\r\nRETURNS int\r\nAS\r\nBEGIN\r\n    DECLARE @s1_len int, @s2_len int\r\n    DECLARE @i int, @j int, @c int, @c_temp int\r\n    DECLARE @cv0 varbinary(max), @cv1 varbinary(max)\r\n    \r\n    SELECT\r\n        @s1_len = LEN(@s1),\r\n        @s2_len = LEN(@s2),\r\n        @cv1 = 0x0000;\r\n    \r\n    WHILE @j <= @s2_len\r\n        SELECT @cv1 = @cv1 + CAST(@j AS binary(2)), @j = @j + 1;\r\n    \r\n    SELECT @i = 1, @cv0 = @cv1, @cv1 = 0x0000;\r\n    \r\n    WHILE @i <= @s1_len\r\n    BEGIN\r\n        SELECT\r\n            @c = @i,\r\n            @cv1 = CAST(@i AS binary(2)),\r\n            @j = 1;\r\n        \r\n        WHILE @j <= @s2_len\r\n        BEGIN\r\n            SET @c = @c + 1;\r\n            SET @c_temp = CAST(SUBSTRING(@cv0, @j+@j-1, 2) AS int) +\r\n                CASE WHEN SUBSTRING(@s1, @i, 1) = SUBSTRING(@s2, @j, 1) THEN 0 ELSE 1 END;\r\n            IF @c > @c_temp SET @c = @c_temp;\r\n            SET @c_temp = CAST(SUBSTRING(@cv0, @j+@j+1, 2) AS int) + 1;\r\n            IF @c > @c_temp SET @c = @c_temp;\r\n            SELECT @cv1 = @cv1 + CAST(@c AS binary(2)), @j = @j + 1;\r\n        END;\r\n        \r\n        SELECT @cv0 = @cv1, @i = @i + 1;\r\n    END;\r\n    \r\n    RETURN CAST(SUBSTRING(@cv1, LEN(@cv1)-1, 2) AS int);\r\nEND;\r\n");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
