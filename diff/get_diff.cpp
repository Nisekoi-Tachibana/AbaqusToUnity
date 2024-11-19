#include <iostream>
#include <fstream>
#include <vector>
#include <algorithm>
#include <sstream>
#include <cmath>

struct Point3D {
    int index;
    double x, y, z;
    
    Point3D(int idx, double x_val, double y_val, double z_val)
        : index(idx), x(x_val), y(y_val), z(z_val) {}
        
    bool operator==(const Point3D& other) const {
        const double epsilon = 1e-6;
        return std::abs(x - other.x) < epsilon &&
               std::abs(y - other.y) < epsilon &&
               std::abs(z - other.z) < epsilon;
    }
};

bool comparePoints(const Point3D& p1, const Point3D& p2) {
    const double epsilon = 1e-6;
    if (std::abs(p1.x - p2.x) > epsilon) return p1.x < p2.x;
    if (std::abs(p1.y - p2.y) > epsilon) return p1.y < p2.y;
    if (std::abs(p1.z - p2.z) > epsilon) return p1.z < p2.z;
    return false;
}

std::vector<Point3D> readFile(const std::string& filename) {
    std::vector<Point3D> points;
    std::ifstream inFile(filename);
    
    if (!inFile.is_open()) {
        std::cerr << "无法打开文件: " << filename << std::endl;
        return points;
    }
    
    std::string line;
    while (std::getline(inFile, line)) {
        std::istringstream iss(line);
        double index_with_decimal; // 先用 double 读取可能带小数点的索引值
        double x, y, z;
        
        if (iss >> index_with_decimal >> x >> y >> z) {
            int index = static_cast<int>(index_with_decimal); // 强制转换为整数
            points.emplace_back(index, x, y, z);
        }
    }
    
    std::cout << "从文件 " << filename << " 读取到 " << points.size() << " 个点" << std::endl;
    return points;
}

int main() {
    std::vector<Point3D> points1 = readFile("D:\\Cpp trash\\node_fb_before.txt");
    std::vector<Point3D> points2 = readFile("D:\\Cpp trash\\node_init_before.txt");
    
    if (points1.empty() || points2.empty()) {
        std::cerr << "文件读取失败或文件为空！" << std::endl;
        return 1;
    }
    
    std::sort(points1.begin(), points1.end(), comparePoints);
    std::sort(points2.begin(), points2.end(), comparePoints);
    
    std::ofstream diffFile1("D:\\Cpp trash\\diff_points1.txt");
    std::ofstream diffFile2("D:\\Cpp trash\\diff_points2.txt");
    
    if (!diffFile1.is_open() || !diffFile2.is_open()) {
        std::cerr << "无法创建输出文件！" << std::endl;
        return 1;
    }
    
    size_t i = 0, j = 0;
    size_t diffCount1 = 0, diffCount2 = 0;
    
    while (i < points1.size() && j < points2.size()) {
        if (comparePoints(points1[i], points2[j])) {
            diffFile1 << points1[i].index << "\t"
                     << points1[i].x << "\t"
                     << points1[i].y << "\t"
                     << points1[i].z << "\n";
            diffCount1++;
            i++;
        }
        else if (comparePoints(points2[j], points1[i])) {
            diffFile2 << points2[j].index << "\t"
                     << points2[j].x << "\t"
                     << points2[j].y << "\t"
                     << points2[j].z << "\n";
            diffCount2++;
            j++;
        }
        else {
            if (!(points1[i] == points2[j])) {
                diffFile1 << points1[i].index << "\t"
                         << points1[i].x << "\t"
                         << points1[i].y << "\t"
                         << points1[i].z << "\n";
                diffFile2 << points2[j].index << "\t"
                         << points2[j].x << "\t"
                         << points2[j].y << "\t"
                         << points2[j].z << "\n";
                diffCount1++;
                diffCount2++;
            }
            i++;
            j++;
        }
    }
    
    while (i < points1.size()) {
        diffFile1 << points1[i].index << "\t"
                 << points1[i].x << "\t"
                 << points1[i].y << "\t"
                 << points1[i].z << "\n";
        diffCount1++;
        i++;
    }
    
    while (j < points2.size()) {
        diffFile2 << points2[j].index << "\t"
                 << points2[j].x << "\t"
                 << points2[j].y << "\t"
                 << points2[j].z << "\n";
        diffCount2++;
        j++;
    }
    
    std::cout << "比较完成！" << std::endl;
    std::cout << "文件1中不同的点数：" << diffCount1 << std::endl;
    std::cout << "文件2中不同的点数：" << diffCount2 << std::endl;
    std::cout << "结果已分别保存" << std::endl;

    
    return 0;
}