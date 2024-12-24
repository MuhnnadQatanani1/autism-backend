# المرحلة الأولى: بناء التطبيق
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# تعيين دليل العمل إلى /src
WORKDIR /src

# نسخ ملف المشروع Autism.csproj إلى الحاوية
COPY ./Autism.csproj ./Autsim/

# استعادة الاعتمادات
RUN dotnet restore "./Autsim/Autism.csproj"

# نسخ باقي الملفات إلى الحاوية
COPY ./ ./

# بناء التطبيق
WORKDIR /src/Autsim
RUN dotnet publish "Autism.csproj" -c Release -o /app

# المرحلة الثانية: نشر التطبيق
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# تعيين دليل العمل إلى /app
WORKDIR /app

# نسخ الملفات المنشورة من مرحلة البناء إلى الحاوية
COPY --from=build /app .

# إعداد نقطة دخول الحاوية لتشغيل التطبيق
ENTRYPOINT ["dotnet", "Autsim.dll"]
