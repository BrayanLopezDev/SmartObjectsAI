#include <string>
#include <iostream>
#define WIN32_LEAN_AND_MEAN
#include <Windows.h>

#include <filesystem>


namespace fs = std::filesystem;

int main(int argc, char* argv[])
{
  std::string basePath = "./";
  std::string exeName = "SmartObjects+.exe";
  std::string additionalArgs = " -window-mode -nographics -batchmode";
  int runs = 10;

    std::string cmd = "start " + exeName + additionalArgs;
    std::cout << cmd << std::endl;


    for (int i = 0; i < runs; ++i)
    {
      system(cmd.c_str());
      Sleep(25000);
    }
  //if (argc > 1)
  //{

    //for (auto& p : fs::directory_iterator(basePath))
    //{
    //  auto cmd = p.path().generic_u8string();
    //  cmd += "/" + exeName;
    //  cmd = std::string{ "start " } + cmd + additionalArgs;
    //  std::cout << cmd << std::endl;
    //  system(cmd.c_str());
    //  
    //}
    //for (int i = 0; i < runsOfEach; ++i)
    //{
    //  for (int j = 2; j < argc; ++j)
    //  {
    //    system((basePath + argv[j] + "/SmartObjects+").c_str());
    //  }
    //  Sleep(2);
    //}
  //}
  return 0;
}