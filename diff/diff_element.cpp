//将不同的节点对应到所属四面体元素

#include <iostream>
#include <fstream>
#include <vector>
#include <set>
#include <sstream>
#include <algorithm>

// 结构体存储四面体元素信息
struct Tetrahedron {
    int index;                  // 四面体的索引编号
    std::vector<int> nodes;     // 10个节点的索引
    
    Tetrahedron(int idx) : index(idx) {
        nodes.reserve(10);  // 预分配10个节点的空间
    }
};

// 读取差异点的索引
std::set<int> readDiffPointIndices(const std::string& filename) {
    std::set<int> diffIndices;
    std::ifstream inFile(filename);
    std::string line;
    
    while (std::getline(inFile, line)) {
        std::istringstream iss(line);
        int index;
        double x, y, z;  // 我们只需要索引值，但要读取完整的一行
        
        if (iss >> index >> x >> y >> z) {
            diffIndices.insert(index);
        }
    }
    
    return diffIndices;
}

// 读取四面体元素信息
std::vector<Tetrahedron> readTetrahedrons(const std::string& filename) {
    std::vector<Tetrahedron> tetras;
    std::ifstream inFile(filename);
    std::string line;
    
    while (std::getline(inFile, line)) {
        std::istringstream iss(line);
        int tetraIndex;
        iss >> tetraIndex;
        
        Tetrahedron tetra(tetraIndex);
        
        // 读取10个节点索引
        int nodeIndex;
        for (int i = 0; i < 10; ++i) {
            if (iss >> nodeIndex) {
                tetra.nodes.push_back(nodeIndex);
            }
        }
        
        if (tetra.nodes.size() == 10) {  // 确保读取了10个节点
            tetras.push_back(tetra);
        }
    }
    
    return tetras;
}

int main() {
    // 读取差异点的索引
    std::set<int> diffIndices = readDiffPointIndices("diff_points2.txt");
    
    // 读取四面体元素信息
    std::vector<Tetrahedron> tetras = readTetrahedrons("element_nodes.txt");
    
    // 创建输出文件
    std::ofstream outFile("affected_tetras.txt");
    
    // 检查每个四面体是否包含差异点
    for (const auto& tetra : tetras) {
        bool containsDiffPoint = false;
        std::vector<int> containedDiffNodes;  // 存储这个四面体包含的差异点索引
        
        // 检查四面体的每个节点
        for (int nodeIndex : tetra.nodes) {
            if (diffIndices.find(nodeIndex) != diffIndices.end()) {
                containsDiffPoint = true;
                containedDiffNodes.push_back(nodeIndex);
            }
        }
        
        // 如果包含差异点，输出这个四面体的信息
        if (containsDiffPoint) {
            // 首先输出四面体的索引
            outFile << tetra.index << "\t";
            
            // 输出所有10个节点的索引
            for (int nodeIndex : tetra.nodes) {
                outFile << nodeIndex << "\t";
            }
            
            // 在行末添加这个四面体包含的差异点信息
            /*outFile << "# 包含差异点: ";
            for (int diffNode : containedDiffNodes) {
                outFile << diffNode << " ";
            }*/
            outFile << "\n";
        }
    }
    
    std::cout << "已找到包含差异点的四面体元素，结果保存在 affected_tetras.txt" << std::endl;
    
    return 0;
}