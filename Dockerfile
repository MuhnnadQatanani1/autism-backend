# المرحلة الأولى: بناء التطبيق
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# نسخ ملف المشروع إلى الحاوية
COPY ["Autsim/Autism.csproj", "Autsim/"]

# استعادة الاعتمادات
RUN dotnet restore "Autsim/Autism.csproj"

# نسخ باقي الملفات إلى الحاوية
COPY . .

# بناء التطبيق
WORKDIR "/src/Autsim"
RUN dotnet build "Autism.csproj" -c Release -o /app/build

# المرحلة الثانية: نشر التطبيق
FROM build AS publish
RUN dotnet publish "Autsim.csproj" -c Release -o /app/publish

# المرحلة الثالثة: إنشاء صورة التشغيل النهائية
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# نسخ الملفات المنشورة من مرحلة النشر
COPY --from=publish /app/publish .

# تحديد المنفذ الذي يستمع عليه التطبيق (اختياري)
EXPOSE 80

# تحديد نقطة البداية لتشغيل التطبيق
ENTRYPOINT ["dotnet", "Autsim.dll"]
