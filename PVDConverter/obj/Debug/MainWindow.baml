   M S B A M L   `   `   `  ����G  CPVDConverter, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:PVDConverterPVDConverter  5   	       PVDConverter.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ,localclr-namespace:PVDConverter     5   	     � Title$  PVD Converter��5   	   $	��450��6   $	��800��6,   .��5	      �  � RowDefinitions 5
   
   �� 5      $	��.5* �6   �� 5      �� 5      $��2* �6   �� 5      �� 5       � ColumnDefinitions 5   
   �� 5      $	��.5* �6   �� 5      �� 5      $	��.5* �6   �� 5      �� 5      $	��.5* �6   .+�5   
   � -   5        $��
Percentage��5      $��2��6(   $��0��65   $��5��6E   $��40q�6Y    � Value 6e   �� *	Percent+ �� UpdateSourceTrigger$ PropertyChanged=��� 5   
   -   5        $��Textout��5      $��2��6   $��5��6,   $��40q�6@   $��40(�6L   $�� ��6Z   $��Center=�6e   �� 5   
   -   5        $
��Open��5      $��1��6   $��1��6*   $��1��6:   $��
Input file��6N   $��24(�6c   $��15q�6q   $��5q�6}   6�   �� 5   
   -   5        $
��Save��5      $��1��6   $��3��6*   $��1��6:   $��Output file��6N   $��24(�6d   $��15q�6r   $��5q�6~   6�   �� 5   
   -   5        $��Start��5      $��3��6   $��1��6+   $��1��6;   $��Start��6O   $��24(�6_   $��15q�6m   $��5q�6y   6�   �� 5   
   -   5        $��Close��5      $��3��6   $��3��6+   $��1��6;   $��Close��6O   $��24(�6_   $��15q�6m   $��5q�6y   6�   5      5      