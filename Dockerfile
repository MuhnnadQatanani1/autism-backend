# المرحلة الأولى: بناء التطبيق
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# نسخ ملف Autism.csproj إلى الحاوية
COPY ./Autism.csproj ./ 

# استعادة الاعتمادات
RUN dotnet restore "Autism.csproj"

# نسخ باقي الملفات إلى الحاوية
COPY ./ ./

# بناء التطبيق
RUN dotnet publish "Autism.csproj" -c Release -o /app/publish

# المرحلة الثانية: بناء صورة التشغيل النهائية
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Autism.dll"]
