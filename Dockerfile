# المرحلة الأولى: بناء التطبيق
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# نسخ ملف المشروع واستعادة الاعتمادات
COPY ./Autism.csproj ./   # تأكد أن هذا المسار صحيح وأن الملف موجود هنا
RUN dotnet restore "Autism.csproj"

# نسخ باقي الملفات وبناء التطبيق
COPY . ./  # نسخ باقي الملفات من مجلد المشروع إلى الحاوية
RUN dotnet build "Autism.csproj" -c Release -o /app/build

# المرحلة الثانية: نشر التطبيق
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app .  # نسخ جميع الملفات من مرحلة البناء
ENTRYPOINT ["dotnet", "Autism.dll"]
