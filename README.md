# SchemaGenerator
F# Schema generator from excel. The code uses LinqToExcel from https://github.com/paulyoder/LinqToExcel.
Learning F# and decided to write a useful tool. This was intended to easily manage data
between non DB Admins and DB Admins. End users may only neeed to write types such as (string/text, date, money, number etc)
and the code can be modified to choose the right DB types either using mappings from excel itself or from other configs.
