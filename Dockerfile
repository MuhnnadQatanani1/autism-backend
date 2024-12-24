# المرحلة الأولى: بناء التطبيق
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# نسخ ملف المشروع واستعادة الاعتمادات
COPY ./Autism.csproj ./  # نسخ ملف Autism.csproj من نفس المجلد الذي يحتوي على Dockerfile
RUN dotnet restore "Autism.csproj"

# نسخ باقي الملفات وبناء التطبيق
COPY . ./  # نسخ جميع الملفات من المجلد الحالي إلى الحاوية
RUN dotnet build "Autism.csproj" -c Release -o /app/build

# المرحلة الثانية: نشر التطبيق
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app .  # نسخ المحتوى من مرحلة البناء
ENTRYPOINT ["dotnet", "Autism.dll"]
